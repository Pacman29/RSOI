using System.Threading.Tasks;
using GRPCService.GRPCProto;

namespace RSOI.Services
{
    public interface IFileService
    {
        Task SaveFile(File file);
        Task GetFile(Path path);
    }
}