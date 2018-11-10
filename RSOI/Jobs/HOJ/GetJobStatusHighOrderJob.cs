using Models.Responses;
using System;
using System.Threading.Tasks;
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
        }
        
        public override async Task ExecuteAsync()
        {
            await DataBaseService.GetJobInfo(this._jobId);
        }
    }
}