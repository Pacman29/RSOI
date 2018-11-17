using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileServer.Jobs;
using Grpc.Core;
using Grpc.Core.Utils;
using GRPCService.GRPCProto;
using JobExecutor;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using File = GRPCService.GRPCProto.File;
using Path = GRPCService.GRPCProto.Path;

namespace FileServer
{
    public class FileServerGrpc : GRPCService.GRPCProto.FileServer.FileServerBase, IDisposable
    {
        private readonly IJobExecutor _jobExecutor;
        private readonly GateWay.GateWayClient _gateWay;
        private readonly Channel _channel;
        private readonly ILogger<FileServerGrpc> _logger;

        public FileServerGrpc()
        {
            _jobExecutor= JobExecutor.JobExecutor.Instance;
            var channelOptions = new List<ChannelOption>();
            channelOptions.Add(new ChannelOption(ChannelOptions.MaxReceiveMessageLength, -1));
            _channel = new Channel("localhost",8001,ChannelCredentials.Insecure,channelOptions);
            _gateWay = new GateWay.GateWayClient(_channel);

            var loggerFactory = new LoggerFactory()
                .AddConsole()
                .AddDebug();
            this._logger = loggerFactory.CreateLogger<FileServerGrpc>();
        }

        private Action<Guid> GetHandleJobOk()
        {
            return async (Guid guid) =>
            {
                _jobExecutor.SetJobStatus(guid,EnumJobStatus.Done);
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

        public override async Task<JobInfo> SaveFile(File request, ServerCallContext context)
        {
            
            var memoryStream = new MemoryStream(request.Bytes.ToByteArray());
            var guid = Guid.NewGuid();
            _logger.LogInformation($"Save File ({JsonConvert.SerializeObject(guid.ToString())})");
            var jobInfo = new JobInfo
            {
                JobStatus = EnumJobStatus.Execute, 
                JobId = guid.ToString()
            };

            try
            {
                var job = new SaveFileJob(request.FilePath.Path_, memoryStream)
                {
                    Guid = guid
                };
                
                _jobExecutor.JobAsyncExecute(job, GetHandleJobOk(), GetHandleJobError());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return jobInfo;
        }

        public override async Task<JobInfo> GetFile(Path request, ServerCallContext context)
        {
            _logger.LogInformation($"Get File ({JsonConvert.SerializeObject(request)})");
            var guid = Guid.NewGuid();
            var jobInfo = new JobInfo
            {
                JobStatus = EnumJobStatus.Execute, 
                JobId = guid.ToString()
            };
                
            try
            {
                var job = new GetFileJob(request.Path_)
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

        public override async Task<JobInfo> GetFiles(Paths request, ServerCallContext context)
        {
            _logger.LogInformation($"Get files ({JsonConvert.SerializeObject(request)})");
            var guid = Guid.NewGuid();
            var jobInfo = new JobInfo
            {
                JobStatus = EnumJobStatus.Execute, 
                JobId = guid.ToString()
            };
                
            try
            {
                var job = new GetFilesJob(request.FilePaths.ToList())
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

        public override async Task<JobInfo> DeleteFile(Path request, ServerCallContext context)
        {
            _logger.LogInformation($"Delete File ({JsonConvert.SerializeObject(request)})");
            var guid = Guid.NewGuid();
            var jobInfo = new JobInfo
            {
                JobStatus = EnumJobStatus.Execute, 
                JobId = guid.ToString()
            };
                
            try
            {
                var job = new DeleteFile(request.Path_)
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

        public async void Dispose()
        {
           await _channel.ShutdownAsync();
        }
    }
}