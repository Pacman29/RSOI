using System;
using System.Threading.Tasks;
using GRPCService.GRPCProto;
using JobExecutor;
using RSOI.Services;

namespace RSOI.Jobs
{
    public class AddFileInfoToDatabaseJob : GateWayJob
    {
        private readonly FileInfo _fileInfo;
        public IDataBaseService DataBaseService { get; set; }

        public AddFileInfoToDatabaseJob(FileInfo fileInfo)
        {
            this._fileInfo = fileInfo;
        }
        
        public override async Task ExecuteAsync()
        {
            var jobInfo = await DataBaseService.CreateFileInfo(this._fileInfo);
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }

        public override async Task Reject()
        {
            Console.WriteLine($"{this.Guid} job reject");
        }
    }
}