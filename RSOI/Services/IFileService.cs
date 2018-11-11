using System.Collections.Generic;
using System.Threading.Tasks;
using GRPCService.GRPCProto;

namespace RSOI.Services
{
    public interface IFileService
    {
        Task<JobInfo> SaveFile(File file);
        Task<JobInfo> GetFile(Path path);
        Task<JobInfo> GetFiles(IEnumerable<string> paths);
        Task<JobInfo> DeleteFile(Path path);
    }
}