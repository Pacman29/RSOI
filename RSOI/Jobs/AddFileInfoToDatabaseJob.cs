using System;
using System.Threading.Tasks;
using GRPCService.GRPCProto;
using JobExecutor;
using RSOI.Services;

namespace RSOI.Jobs
{
    public class AddFileInfoToDatabaseJob : BaseJob
    {
        private FileInfo _fileInfo;
        private IDataBaseService _dataBaseService;

        public AddFileInfoToDatabaseJob(Guid jobId,IDataBaseService dataBaseService, FileInfo fileInfo, BaseJob rootJob = null)
        {
            this._fileInfo = fileInfo;
            this._dataBaseService = dataBaseService;
            this.Guid = jobId;
            this.RootJob = rootJob;
        }
        
        public override async Task ExecuteAsync()
        {
            var jobInfo = await _dataBaseService.CreateFileInfo(_fileInfo);
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }

        public override async Task Reject()
        {
            Console.WriteLine($"{this.Guid} job reject");
        }
    }
}