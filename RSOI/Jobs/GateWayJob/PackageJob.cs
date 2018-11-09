using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using GRPCService.GRPCProto;
using JobExecutor;

namespace RSOI.Jobs
{
    public class PackageJob : BaseJob
    {
        private ConcurrentDictionary<Guid, BaseJob> _jobs = new ConcurrentDictionary<Guid, BaseJob>();
        private int completeCounter = 0;
        private readonly object threadLock;

        public PackageJob(BaseJob[] jobs)
        {
            foreach (var job in jobs)
                SetEventAndAdd(job);
        }

        public PackageJob(BaseJob job) : this(new BaseJob[]{job}) {}
        public PackageJob() : this(new BaseJob[0]) {}

        private void SetEventAndAdd(BaseJob job)
        {
            lock(threadLock)
                completeCounter++;
            job.OnDone += async j => 
            {
                lock(threadLock)
                    completeCounter--;
                CheckCompleat();
            };
        }

        public void AddJob(BaseJob job)
        {
            SetEventAndAdd(job);
        }
        
        private void CheckCompleat()
        {
            lock (threadLock)
                if(completeCounter == 0)
                    this.JobStatus = EnumJobStatus.Done;
        }

        public override async Task ExecuteAsync()
        {
            foreach (var keyValuePair in _jobs)
                this.Executor.JobAsyncExecute(keyValuePair.Value);
        }

        public PackageJob RunNextOnDone(BaseJob nextJob)
        {
            this.OnDone += async job =>
            {
                this.Executor.JobAsyncExecute(nextJob);
            };
            return this;
        }

        public PackageJob RunNextOnDone(BaseJob nextJob, Func<BaseJob, BaseJob, Task> beforeNextJob)
        {
            this.OnDone += async job =>
            {
                await beforeNextJob.Invoke(job, nextJob);
                this.Executor.JobAsyncExecute(nextJob);
            };
            return this;
        }

        public PackageJob RunNextOnDone(Func<BaseJob, Task<BaseJob>> beforeNextJob)
        {
            this.OnDone += async job =>
            {
                this.Executor.JobAsyncExecute(await beforeNextJob.Invoke(job));
            };
            return this;
        }
    }
}