using System;
using System.Threading.Tasks;
using GRPCService.GRPCProto;

namespace RSOI.Services
{
    public interface IDataBaseService : IDisposable
    {
        Task<JobInfo> CreateFileInfo(FileInfo pdfFileInfo);
        Task<JobInfo> UpdateOrCreateJob(JobInfo jobInfo);
        Task DoneJob(Guid jobId);
    }
}