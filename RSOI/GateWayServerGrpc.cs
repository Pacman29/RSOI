using System.Threading.Tasks;
using Grpc.Core;
using GRPCService.GRPCProto;
using RSOI.Services.Impl;

namespace RSOI
{
    public class GateWayServerGrpc : GateWay.GateWayBase
    {
        private JobExecutor.JobExecutor _jobExecutor = JobExecutor.JobExecutor.Instance;

        public override async Task<Empty> PostJobInfo(JobInfo request, ServerCallContext context)
        {
            
            return new Empty();
        }
    }
}