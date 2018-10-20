using System;
using System.Threading.Tasks;

namespace JobExecutor
{
    public interface IJobExecutor
    {
        void AddJob(BaseJob job,Action<Guid> onOk,Action<Guid,Exception> onError);
        void AddJob(BaseJob job);
        BaseJob GetJob(Guid id);
        BaseJob DeleteJob(Guid id);
    }
}