using Models.Responses;
using System;
using System.Threading.Tasks;

namespace RSOI.Jobs
{
    public class GetJobStatusHighOrderJob : GateWayJob<JobInfo>
    {
        
        public GetJobStatusHighOrderJob(string JobId)
        {
        }

        public override Task ExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }
}