using System.Threading.Tasks;
using GRPCService.GRPCProto;

namespace RSOI.Services
{
    public interface IFileService
    {
        Task<JobInfo> SaveFile(File file);
        Task<JobInfo> GetFile(Path path);
    }
}