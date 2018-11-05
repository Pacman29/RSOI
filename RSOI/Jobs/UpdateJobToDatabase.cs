using GRPCService.GRPCProto;
using JobExecutor;
using RSOI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSOI.Jobs
{
    public class UpdateJobToDatabase : BaseJob
    {
        private IDataBaseService _dataBaseService;
        private readonly string jobId;
        private readonly EnumJobStatus status;

        public UpdateJobToDatabase(IDataBaseService dataBaseService, string JobId, EnumJobStatus status)
        {
            _dataBaseService = dataBaseService;
            jobId = JobId;
            this.status = status;
        }

        public override async Task ExecuteAsync()
        {
            var jobInfo = await _dataBaseService.UpdateOrCreateJob(new JobInfo()
            {
                JobId = jobId,
                JobStatus = status
            });
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }
    }
}
