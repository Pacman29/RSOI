using System;
using System.Threading.Tasks;
using Grpc.Core;
using GRPCService.GRPCProto;
using JobExecutor;
using RSOI.Services.Impl;

namespace RSOI
{
    public class GateWayServerGrpc : GateWay.GateWayBase
    {
        private readonly IJobExecutor _jobExecutor = JobExecutor.JobExecutor.Instance;

        public override async Task<Empty> PostJobInfo(JobInfo request, ServerCallContext context)
        {
            switch (request.JobStatus)
            {
                    case EnumJobStatus.Execute: 
                        Console.WriteLine($"{request.JobId} execute");
                        break;
                    case EnumJobStatus.Done:
                        Console.WriteLine($"{request.JobId} done");
                        break;
                    case EnumJobStatus.Error:
                        Console.WriteLine($"{request.JobId} error; {request.Message}");
                        break;
            }
            _jobExecutor.SetJobStatus(new Guid(request.JobId),request.JobStatus);
            return new Empty();
        }
    }
}