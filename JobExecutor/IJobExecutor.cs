using System;
using System.Threading.Tasks;
using GRPCService.GRPCProto;

namespace JobExecutor
{
    public interface IJobExecutor
    {
        void AddJob(BaseJob job,Action<Guid> onOk,Action<Guid,Exception> onError);
        void AddJob(BaseJob job);
        BaseJob GetJob(Guid id);
        BaseJob DeleteJob(Guid id);
        Task<BaseJob> RejectJobAsync(Guid id);
        Task<BaseJob> DoneJobAsync(Guid id);
        void SetJobStatus(Guid id, EnumJobStatus status);
    }
}