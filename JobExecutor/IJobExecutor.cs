using System;
using System.Threading.Tasks;
using GRPCService.GRPCProto;

namespace JobExecutor
{
    public interface IJobExecutor
    {
        void JobAsyncExecute(BaseJob job,Action<Guid> onOk,Action<Guid,Exception> onError);
        void JobAsyncExecute(BaseJob job);
        void JobExecute(BaseJob job, Action<Guid> onOk, Action<Guid, Exception> onError);
        void JobExecute(BaseJob job);
        BaseJob GetJob(Guid id);
        BaseJob DeleteJob(Guid id);
        Task<BaseJob> RejectJobAsync(Guid id);
        Task<BaseJob> DoneJobAsync(Guid id);
        void SetJobStatus(Guid id, EnumJobStatus status, byte[] bytes = null);
        void SetJobStatus(Guid id, EnumJobStatus status);
        bool SetJobStatusByServiceGuid(Guid id, EnumJobStatus status);
        bool SetJobStatusByServiceGuid(Guid id, EnumJobStatus status, byte[] bytes = null);
    }
}