using Models.Responses;
using System;
using System.Threading.Tasks;
using JobExecutor;
using RSOI.Services;

namespace RSOI.Jobs
{
    public class GetJobStatusHighOrderJob : GateWayJob<JobInfo>
    {
        public IDataBaseService DataBaseService { get; set; }
        private string _jobId;

        public GetJobStatusHighOrderJob(string JobId)
        {
            this._jobId = JobId;
            
            this.OnDone += async job =>
            {
                this.InvokeOnHaveResult(BytesDeserializer<JobInfo>.Deserialize(job));
            };
        }
        
        public override async Task ExecuteAsync()
        {
            var jobInfo = await DataBaseService.GetJobInfo(this._jobId);
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }
    }
}