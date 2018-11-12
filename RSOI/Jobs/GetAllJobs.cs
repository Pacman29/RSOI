using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JobExecutor;
using Models.Responses;
using RSOI.Services;

namespace RSOI.Jobs
{
    public class GetAllJobs : GateWayJob<List<JobInfoBase>>
    {
        public IDataBaseService DataBaseService { get; set; }
        
        public GetAllJobs()
        {
            this.OnDone += async job =>
            {
                this.InvokeOnHaveResult(BytesDeserializer<List<JobInfoBase>>.Deserialize(job));
            };
        }

        public override async Task ExecuteAsync()
        {
            var jobInfo = await DataBaseService.GetAllJobInfos();
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }
    }
}