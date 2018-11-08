using System;
using System.Threading.Tasks;
using JobExecutor;

namespace RSOI.Jobs
{
    public class GateWayJob : BaseJob
    {
        public GateWayJob RunNext(GateWayJob nextJob)
        {
            this.OnDone += job =>
            {
                this.Executor.JobAsyncExecute(nextJob);
            };
            return this;
        }
        
        public GateWayJob RunNext(GateWayJob nextJob, Func<BaseJob,GateWayJob,Task> beforeNextJob)
        {
            this.OnDone += async job =>
            {
                await beforeNextJob.Invoke(job, nextJob);
                this.Executor.JobAsyncExecute(nextJob);
            };
            return this;
        }
        
        public GateWayJob RunNext(Func<BaseJob,Task<GateWayJob>> beforeNextJob)
        {
            this.OnDone += async job =>
            {
                this.Executor.JobAsyncExecute(await beforeNextJob.Invoke(job));
            };
            return this;
        }
        
    }
}