using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Responses;
using RSOI.Services;

namespace RSOI.Jobs
{
    public class GetAllJobInfosHighOrderJob : GateWayJob<List<JobInfoBase>>
    {
        public IGateWayJobsFabric GateWayJobsFabric { get; set; }
        
        public GetAllJobInfosHighOrderJob()
        {
        }

        public override async Task ExecuteAsync()
        {
            var getAllJobs = GateWayJobsFabric.GetAllJobs();
            getAllJobs.OnHaveResult += async result => { this.InvokeOnHaveResult(result); };  
            this.Executor.JobAsyncExecute(getAllJobs);
        }
    }
}