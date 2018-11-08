using GRPCService.GRPCProto;
using JobExecutor;
using RSOI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSOI.Jobs
{
    public class UpdateJobToDatabase : GateWayJob
    {
        public IDataBaseService DataBaseService { get; set; }
        private readonly string jobId;
        private readonly EnumJobStatus status;

        public UpdateJobToDatabase(string JobId, EnumJobStatus status)
        {
            jobId = JobId;
            this.status = status;
        }

        public override async Task ExecuteAsync()
        {
            var jobInfo = await DataBaseService.UpdateOrCreateJob(new JobInfo()
            {
                JobId = jobId,
                JobStatus = status
            });
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }
    }
}
