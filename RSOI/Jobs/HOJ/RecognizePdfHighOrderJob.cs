using GRPCService.GRPCProto;
using Models.Responses;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Path = System.IO.Path;
using PdfFile = Models.Requests.PdfFile;

namespace RSOI.Jobs
{
    public class RecognizePdfHighOrderJob : GateWayJob<Models.Responses.JobInfo>
    {
        
        private readonly object _threadLock = new object();
        private readonly PdfFile _pdfFile;
        public IGateWayJobsFabric GateWayJobsFabric { get; set; }

        public RecognizePdfHighOrderJob(PdfFile pdfFile)
        {
            _pdfFile = pdfFile;
        }


        public override async Task ExecuteAsync()
        {
            
            //Create job to database
            var createJobToDatabaseJob = GateWayJobsFabric.GetCreateJobToDatabase();
            createJobToDatabaseJob.OnHaveResult += async result =>
            {
                this.InvokeOnHaveResult(new Models.Responses.JobInfo()
                {
                    JobId = result,
                    JobStatus = EnumJobStatus.Execute
                });
            };
            
            var pdfFileInfo = await _pdfFile.GetFileInfo();
            var bytes = await _pdfFile.ReadFile();
           
            createJobToDatabaseJob.RunNextOnHaveResult(async (jobId) =>
            {              
                pdfFileInfo.Path = $"{jobId}.pdf";
                pdfFileInfo.JobId = jobId;
                var pdfPackageJob = GateWayJobsFabric.GetPackageJob();
                var addPdfToDatabaseJob = 
                    GateWayJobsFabric.GetAddFileInfoToDatabaseJob(pdfFileInfo);
                pdfPackageJob.AddJob(addPdfToDatabaseJob);
                var savePdfFileJob = 
                    GateWayJobsFabric.GetSaveFileJob(bytes, pdfFileInfo.Path);
                pdfPackageJob.AddJob(savePdfFileJob);
                var recognizePdfFileJob =
                    GateWayJobsFabric.GetRecognizePdfFileJob(bytes, new List<int>());
                pdfPackageJob.AddJob(recognizePdfFileJob);
                
                recognizePdfFileJob.RunNextOnHaveResult(async (zipImgs) =>
                {
                    var imagesPackageJob = GateWayJobsFabric.GetPackageJob();
                    foreach (var image in zipImgs.Entries)
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
                    imagesPackageJob.RunNextOnDone(updateStatusJob);
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