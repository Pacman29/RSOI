using System;
using System.Threading.Tasks;
using JobExecutor;

namespace RSOI.Jobs
{
    public class GateWayJob<TResult> : BaseJob
    {

        public GateWayJob<TResult> RunNextOnDone(BaseJob nextJob)
        {
            this.OnDone += job =>
            {
                this.Executor.JobAsyncExecute(nextJob);
            };
            return this;
        }
        
        public GateWayJob<TResult> RunNextOnDone(BaseJob nextJob, Func<BaseJob,BaseJob,Task> beforeNextJob)
        {
            this.OnDone += async job =>
            {
                await beforeNextJob.Invoke(job, nextJob);
                this.Executor.JobAsyncExecute(nextJob);
            };
            return this;
        }
        
        public GateWayJob<TResult> RunNextOnDone(Func<BaseJob,Task<BaseJob>> beforeNextJob)
        {
            this.OnDone += async job =>
            {
                this.Executor.JobAsyncExecute(await beforeNextJob.Invoke(job));
            };
            return this;
        }

        public GateWayJob<TResult> RunNextOnHaveResult(BaseJob nextJob)
        {
            this.OnHaveResult += async result =>
            {
                this.Executor.JobAsyncExecute(nextJob);
            };
            return this;
        }

        public GateWayJob<TResult> RunNextOnHaveResult(BaseJob nextJob, Func<TResult, BaseJob, Task> beforeNextJob)
        {
            this.OnHaveResult += async result =>
            {
                await beforeNextJob.Invoke(result, nextJob);
                this.Executor.JobAsyncExecute(nextJob);
            };
            return this;
        }

        public GateWayJob<TResult> RunNextOnHaveResult(Func<TResult, Task<BaseJob>> beforeNextJob)
        {
            this.OnHaveResult += async result =>
            {
                this.Executor.JobAsyncExecute(await beforeNextJob.Invoke(result));
            };
            return this;
        }

        public delegate Task GateWayJobEvent(TResult result);

        public event GateWayJobEvent OnHaveResult;

        public async void InvokeOnHaveResult(TResult result)
        {
            await this.OnHaveResult?.Invoke(result);
        }
    }
}