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
    }
}