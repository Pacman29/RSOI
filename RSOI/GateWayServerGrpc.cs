using System;
using System.Threading.Tasks;
using Grpc.Core;
using GRPCService.GRPCProto;
using JobExecutor;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RSOI.Services.Impl;

namespace RSOI
{
    public class GateWayServerGrpc : GateWay.GateWayBase
    {
        public GateWayServerGrpc()
        {
            var loggerFactory = new LoggerFactory()
                .AddConsole()
                .AddDebug();
            this._logger = loggerFactory.CreateLogger<GateWayServerGrpc>();
        }

        private readonly IJobExecutor _jobExecutor = JobExecutor.JobExecutor.Instance;
        private readonly ILogger _logger;

        private string GetMessageInfo(JobInfo jobInfo)
        {
            return JsonConvert.SerializeObject(jobInfo);
        }

        public override async Task<Empty> PostJobInfo(JobInfo request, ServerCallContext context)
        {
            _logger.LogInformation(GetMessageInfo(request));
            var res = _jobExecutor.SetJobStatusByServiceGuid(new Guid(request.JobId), request.JobStatus);
            while (res == false)
            {
                _logger.LogWarning($"{request.JobId} status not set");
                await Task.Delay(TimeSpan.FromSeconds(5));
                res = _jobExecutor.SetJobStatusByServiceGuid(new Guid(request.JobId), request.JobStatus);
            }
            return new Empty();
        }
        
        public override async Task<Empty> PostJobInfoWithBytes(JobInfoWithBytes request, ServerCallContext context)
        {
            _logger.LogInformation(GetMessageInfo(request.JobInfo));

            var res = _jobExecutor.SetJobStatusByServiceGuid(new Guid(request.JobInfo.JobId),request.JobInfo.JobStatus, request.Bytes.ToByteArray());
            while (res == false)
            {
                _logger.LogWarning($"{request.JobInfo.JobId} status not set");
                await Task.Delay(TimeSpan.FromSeconds(5));
                res = _jobExecutor.SetJobStatusByServiceGuid(new Guid(request.JobInfo.JobId), request.JobInfo.JobStatus, request.Bytes.ToByteArray());
            }
            return new Empty();
        }
    }
}