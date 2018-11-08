using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using GRPCService.GRPCProto;

namespace RSOI.Jobs
{
    public class PackageJob : GateWayJob
    {
        private ConcurrentDictionary<Guid,GateWayJob> _jobs = new ConcurrentDictionary<Guid, GateWayJob>();
        
        
        public PackageJob(GateWayJob[] jobs)
        {
            foreach (var job in jobs)
            {
                if (job.Guid == null) continue;
                _jobs.TryAdd((Guid) job.Guid, job);
                job.OnDone += async j => { CheckCompleat(); };

            }
        }

        private void CheckCompleat()
        {
            if (_jobs.All(pair => pair.Value.JobStatus == EnumJobStatus.Done))
                this.JobStatus = EnumJobStatus.Done;
        }

        public override async Task ExecuteAsync()
        {
            foreach (var keyValuePair in _jobs)
                this.Executor.JobAsyncExecute(keyValuePair.Value);
        }
    }
}