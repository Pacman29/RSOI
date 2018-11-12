using System;
using System.Threading.Tasks;
using JobExecutor;
using RSOI.Services;

namespace RSOI.Jobs
{
    public class DeleteJobFromDatabase : GateWayJob<bool>
    {
        private readonly string _jobId;
        public IDataBaseService DataBaseService { get; set; }

        public DeleteJobFromDatabase(string jobId)
        {
            _jobId = jobId;

            this.OnDone += async (job) =>
            {
                this.InvokeOnHaveResult(BytesDeserializer<bool>.Deserialize(job));
            };
        }

        public override async Task ExecuteAsync()
        {
            var jobInfo = await DataBaseService.DeleteJobInfo(this._jobId);
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }
    }
}