using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using GRPCService.GRPCProto;
using JobExecutor;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RecognizePdfServer.Jobs;

namespace RecognizePdfServer
{
    public class PdfRecognizeServerGrpc : Recognize.RecognizeBase, IDisposable
    {
        private readonly IJobExecutor _jobExecutor;
        private readonly GateWay.GateWayClient _gateWay;
        private readonly Channel _channel;
        private readonly ILogger<PdfRecognizeServerGrpc> _logger;

        public PdfRecognizeServerGrpc()
        {
            _jobExecutor= JobExecutor.JobExecutor.Instance;
            var channelOptions = new List<ChannelOption>();
            channelOptions.Add(new ChannelOption(ChannelOptions.MaxReceiveMessageLength, -1));
            _channel = new Channel("localhost",8001,ChannelCredentials.Insecure,channelOptions);
            _gateWay = new GateWay.GateWayClient(_channel);

            var loggerFactory = new LoggerFactory()
                .AddConsole()
                .AddDebug();
            this._logger = loggerFactory.CreateLogger<PdfRecognizeServerGrpc>();
        }

        private Action<Guid> GetHandleJobOk()
        {
            return async (Guid guid) =>
            {
                _jobExecutor.SetJobStatus(guid, EnumJobStatus.Done);
                this._logger.LogInformation($"OkResponse {guid}");
                var job = _jobExecutor.GetJob(guid);
                if (job.Bytes == null)
                    await _gateWay.PostJobInfoAsync(job.GetJobInfo());
                else
                    await _gateWay.PostJobInfoWithBytesAsync(job.GetJobInfoWithBytes());
            };
        }

        private Action<Guid, Exception> GetHandleJobError()
        {
            return async (Guid guid, Exception e) =>
            {
                this._logger.LogError(e.Message);
                _jobExecutor.SetJobStatus(guid, EnumJobStatus.Error);
                this._logger.LogError($"ErrorResponse {guid}");
                var jobInfo = _jobExecutor.GetJob(guid).GetJobInfo();
                jobInfo.Message = e.ToString();
                await _gateWay.PostJobInfoAsync(jobInfo);
            };
        }
        


        public override async Task<JobInfo> RecognizePdf(PdfFile request, ServerCallContext context)
        {
            
            var memoryStream = new MemoryStream(request.Bytes.ToByteArray());
            var pages = request.Pages.ToArray();

            var guid = Guid.NewGuid();
            _logger.LogInformation($"Recognize pdf ({JsonConvert.SerializeObject(guid.ToString())})");

            var jobInfo = new JobInfo
            {
                JobStatus = EnumJobStatus.Execute, 
                JobId = guid.ToString()
            };
            
            try
            {
                var job = new RecognizePdfJob(memoryStream,pages)
                {
                    Guid = guid
                };
                _jobExecutor.JobExecute(job, GetHandleJobOk(), GetHandleJobError());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            return jobInfo;
        }
        
        public override async Task<Empty> RejectJobCall(RejectJob request, ServerCallContext context)
        {
            var result = new Empty();
            try
            {
                var guid = new Guid(request.JobId);
                var job = await _jobExecutor.RejectJobAsync(guid);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;
        }

        public override async Task<Empty> DoneJobCall(DoneJob request, ServerCallContext context)
        {
            var result = new Empty();
            try
            {
                var guid = new Guid(request.JobId);
                var job = await _jobExecutor.DoneJobAsync(guid);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;
        }

        public async void Dispose()
        {
           await _channel.ShutdownAsync();
        }
    }
}