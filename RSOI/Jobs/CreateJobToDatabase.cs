using System;
using System.Threading.Tasks;
using GRPCService.GRPCProto;
using JobExecutor;
using RSOI.Services;

namespace RSOI.Jobs
{
    public class CreateJobToDatabase : BaseJob
    {
        private IDataBaseService _dataBaseService;

        public CreateJobToDatabase(IDataBaseService dataBaseService)
        {
            _dataBaseService = dataBaseService;
        }

        public override async Task ExecuteAsync()
        {
            var jobInfo = await _dataBaseService.UpdateOrCreateJob(new JobInfo());
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }
    }
}