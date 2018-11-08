using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GRPCService.GRPCProto;
using JobExecutor;
using RSOI.Services;
using Path = System.IO.Path;
using PdfFile = Models.Requests.PdfFile;

namespace RSOI.Jobs
{
    public class RecognizePdfRootJob : GateWayJob
    {

        public new byte[] Bytes
        {
            get
            {
                lock (_threadLock)
                {
                    return base.Bytes;
                }
            }
            set
            {
                lock (_threadLock)
                {
                    base.Bytes = value;
                }
            }
        }
        
        private readonly object _threadLock = new object();
        private readonly PdfFile _pdfFile;
        public IGateWayJobsFabric GateWayJobsFabric { get; set; }

        public RecognizePdfRootJob(PdfFile pdfFile)
        {
            _pdfFile = pdfFile;
        }


        public override async Task ExecuteAsync()
        {
            //Create job to database
            var createJobToDatabaseJob = GateWayJobsFabric.GetCreateJobToDatabase();
           
            createJobToDatabaseJob.RunNext(async (job) =>
            {
                string jobId;
                var bf = new BinaryFormatter();
                using (var ms = new MemoryStream(job.Bytes))
                    jobId = (string) bf.Deserialize(ms);

                this.Bytes = job.Bytes;
                
                var pdfFileInfo = await _pdfFile.GetFileInfo();
                pdfFileInfo.Path = $"{jobId}.pdf";
                pdfFileInfo.JobId = jobId;
                var pdfPackageJob = GateWayJobsFabric.GetPackageJob();
                var addPdfToDatabaseJob = 
                    GateWayJobsFabric.GetAddFileInfoToDatabaseJob(pdfFileInfo);
                pdfPackageJob.AddJob(addPdfToDatabaseJob);
                var savePdfFileJob = 
                    GateWayJobsFabric.GetSaveFileJob(await _pdfFile.ReadFile(), pdfFileInfo.Path);
                pdfPackageJob.AddJob(savePdfFileJob);
                var recognizePdfFileJob =
                    GateWayJobsFabric.GetRecognizePdfFileJob(await _pdfFile.ReadFile(), new List<int>());
                pdfPackageJob.AddJob(recognizePdfFileJob);
                
                recognizePdfFileJob.RunNext(async (recPdfJob) =>
                {
                    var imagesPackageJob = GateWayJobsFabric.GetPackageJob();
                    var zipArch = new ZipArchive(new MemoryStream(recPdfJob.Bytes));
                    foreach (var image in zipArch.Entries)
                    {
                        using (var ms = new MemoryStream())
                        {
                            image.Open().CopyTo(ms);

                            var sBuilder = new StringBuilder();
                            using (var md5Hash = MD5.Create())
                            {
                                var hash = md5Hash.ComputeHash(ms.ToArray());
                                foreach (var data in hash)
                                    sBuilder.Append(data.ToString("x2"));
                            }

                            var fileInfo = new GRPCService.GRPCProto.FileInfo()
                            {
                                FileLength = ms.Length,
                                FileType = GRPCService.GRPCProto.EnumFileType.Image,
                                JobId = jobId,
                                MD5 = sBuilder.ToString(),
                                Path = $"{jobId}_{image.FullName}",
                                PageNo = long.Parse(Path.GetFileNameWithoutExtension(image.Name))
                            };
                            var addImageFileToDatabaseJob = 
                                GateWayJobsFabric.GetAddFileInfoToDatabaseJob(fileInfo);

                            var saveImageFileJob =
                                GateWayJobsFabric.GetSaveFileJob(ms.ToArray(), fileInfo.Path);
                            
                            imagesPackageJob.AddJob(addImageFileToDatabaseJob);
                            imagesPackageJob.AddJob(saveImageFileJob);
                        }
                    }

                    var updateStatusJob = GateWayJobsFabric.GetUpdateJobToDatabase(jobId, EnumJobStatus.Done);
                    imagesPackageJob.RunNext(updateStatusJob);
                    return imagesPackageJob;
                });
                return pdfPackageJob;
            });
            this.Executor.JobAsyncExecute(createJobToDatabaseJob);
        }

        public override Task Reject()
        {
            return base.Reject();
        }
    }
}