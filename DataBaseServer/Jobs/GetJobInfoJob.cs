using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using DataBaseServer.Contexts;
using GRPCService.GRPCProto;
using JobExecutor;
using Models.Responses;
using JobInfo = Models.Responses.JobInfo;

namespace DataBaseServer.Jobs
{
    public class GetJobInfoJob : BaseJob
    {
        private readonly JobsDbManager _jobDbManager;
        private string _jobId;

        public GetJobInfoJob(JobsDbManager jobsDbManager,string jobId)
        {
            this._jobDbManager = jobsDbManager;
            this._jobId = jobId;
        }

        public override async Task ExecuteAsync()
        {
            var files = await _jobDbManager.FindInJobAndFileInfoJoin(this._jobId);
            if (files.Count > 0)
            {
                var jobInfo = new JobInfo
                {
                    JobId = files[0].Guid,
                    JobStatus = files[0].JobStatus.ToString()
                };
                
                if (files[0].JobStatus == EnumJobStatus.Done)
                {
                    var pdfInfo = files.First(fileInfo => fileInfo.FileType == EnumFileType.Pdf);
                    jobInfo.PdfPath = pdfInfo.Path;
                    jobInfo.PdfId = pdfInfo.Id;
                    jobInfo.Images = files.Where(fileInfo => fileInfo.FileType == EnumFileType.Image)
                        .Select(fileInfo => new ImageResponseModel()
                        {
                            PageNo = fileInfo.PageNo,
                            Path = fileInfo.Path,
                            ImgId = fileInfo.Id
                        }).ToList();
                }
                var formatter = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    formatter.Serialize(ms, jobInfo);
                    this.Bytes = ms.ToArray();
                }
            }
        }
    }
}