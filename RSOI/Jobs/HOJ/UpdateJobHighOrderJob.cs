using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GRPCService.GRPCProto;
using Path = GRPCService.GRPCProto.Path;
using PdfFile = Models.Requests.PdfFile;

namespace RSOI.Jobs
{
    public class UpdateJobHighOrderJob : GateWayJob<Models.Responses.JobInfo>
    {
        private readonly PdfFile _pdfFile;
        private readonly string _jobId;
        public IGateWayJobsFabric GateWayJobsFabric { get; set; }

        public UpdateJobHighOrderJob(PdfFile pdfFile, string jobId)
        {
            _pdfFile = pdfFile;
            _jobId = jobId;
        }

        public override async Task ExecuteAsync()
        {
            var getJobsStatusHOJ = GateWayJobsFabric.GetJobStatusHighOrderJob(this._jobId);
            getJobsStatusHOJ.OnHaveResult += async (jobInfo) =>
            {
                if (jobInfo == null)
                {
                    this.InvokeOnHaveError(new JobError()
                    {
                        Message = "id is incorrect",
                        StatusCode = 400
                    });
                }
                else
                {
                    if (jobInfo.JobStatus != "Done")
                    {
                        this.InvokeOnHaveError(new JobError()
                        {
                            Message = "Job execute",
                            StatusCode = 409
                        });
                    }
                    else
                    {
                        var updateJobStatus =
                            GateWayJobsFabric.GetUpdateJobToDatabase(this._jobId, EnumJobStatus.Execute);

                        updateJobStatus.RunNextOnHaveResult(async jobId =>
                        {
                            var pdfFileInfo = await _pdfFile.GetFileInfo();
                            var bytes = await _pdfFile.ReadFile();
                            pdfFileInfo.Path = $"{jobId}.pdf";
                            pdfFileInfo.JobId = jobId;
                            
                            this.InvokeOnHaveResult(new Models.Responses.JobInfo()
                            {
                                JobId = jobId,
                                JobStatus = EnumJobStatus.Execute.ToString()
                            });
                            
                            var deleteAll = GateWayJobsFabric.GetPackageJob();

                            var deletePdfInfo =
                                GateWayJobsFabric.DeleteFileInfoFromDatabaseJob(this._jobId, jobInfo.PdfId, jobInfo.PdfPath,
                                    EnumFileType.Pdf);
                            var deletePdfFile = GateWayJobsFabric.DeleteFileJob(jobInfo.PdfPath);
                            deleteAll.AddJob(deletePdfInfo);
                            deleteAll.AddJob(deletePdfFile);
                            foreach (var img in jobInfo.Images)
                            {
                                var deleteImgInfo =
                                    GateWayJobsFabric.DeleteFileInfoFromDatabaseJob(this._jobId, img.ImgId, img.Path,
                                        EnumFileType.Image);
                                var deleteImgFile = GateWayJobsFabric.DeleteFileJob(img.Path);
                                deleteAll.AddJob(deleteImgInfo);
                                deleteAll.AddJob(deleteImgFile);
                            }

                            deleteAll.RunNextOnDone(async (job) =>
                            {
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
                                                PageNo = long.Parse(System.IO.Path.GetFileNameWithoutExtension(image.Name))
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
                            
                            return deleteAll;
                        });
                        
                        this.Executor.JobAsyncExecute(updateJobStatus);
                    } 
                }
                    
            };
            this.Executor.JobAsyncExecute(getJobsStatusHOJ);
        }
    }
}