using System;
using System.Threading.Tasks;
using GRPCService.GRPCProto;
using JobExecutor;
using RSOI.Services;

namespace RSOI.Jobs
{
    public class CreateJobToDatabase : GateWayJob
    {
        public IDataBaseService DataBaseService { get; set; }

        public override async Task ExecuteAsync()
        {
            var jobInfo = await DataBaseService.UpdateOrCreateJob(new JobInfo());
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }
    }
}