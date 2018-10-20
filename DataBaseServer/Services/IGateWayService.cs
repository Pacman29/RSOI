using System;
using System.Threading.Tasks;
using GRPCService.GRPCProto;

namespace DataBaseServer.Services
{
    public interface IGateWayService : IDisposable
    {
        Task SendJobInfo(JobInfo jobInfo);
    }
}