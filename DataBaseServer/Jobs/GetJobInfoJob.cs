using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using DataBaseServer.Contexts;
using GRPCService.GRPCProto;
using JobExecutor;
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
            var job = await _jobDbManager.FindByGuid(this._jobId);
            if (job != null)
            {
                var jobInfo = new JobInfo
                {
                    JobId = job.GUID,
                    JobStatus = job.status
                };
                
                if (job.status == EnumJobStatus.Done)
                {
                    jobInfo.PdfPath = job.fileInfos.First(fileInfo => fileInfo.FileType == EnumFileType.Pdf).Path;
                    jobInfo.ImagePath = job.fileInfos.Where(fileInfo => fileInfo.FileType == EnumFileType.Image)
                        .Select(fileInfo => fileInfo.Path).ToArray();
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