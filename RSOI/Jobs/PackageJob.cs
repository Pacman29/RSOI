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
                SetEventAndAdd(job);
        }

        public PackageJob(GateWayJob job) : this(new GateWayJob[]{job}) {}
        public PackageJob() : this(new GateWayJob[0]) {}

        private void SetEventAndAdd(GateWayJob job)
        {
            if (job.Guid == null)
                return;
            _jobs.TryAdd((Guid) job.Guid, job);
            job.OnDone += async j => { CheckCompleat(); };
        }

        public void AddJob(GateWayJob job)
        {
            SetEventAndAdd(job);
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