using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataBaseServer.Contexts;
using DataBaseServer.DBO;
using DataBaseServer.Exceptions.DBExceptions;
using DataBaseServer.Jobs;
using Grpc.Core;
using GRPCService.GRPCProto;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FileInfo = GRPCService.GRPCProto.FileInfo;

namespace DataBaseServer
{
    public class DataBaseServerGrpc : DataBase.DataBaseBase, IDisposable
    {
        private readonly FileInfosDbManager _fileInfosDbManager;
        private readonly JobExecutor.IJobExecutor _jobExecutor;
        private readonly GateWay.GateWayClient _gateWay;
        private readonly Channel _channel;
        private readonly JobsDbManager _jobsDbManager;
        private readonly ILogger<DataBaseServerGrpc> _logger;

        public DataBaseServerGrpc() : base()
        {
            _fileInfosDbManager = new FileInfosDbManager(new BaseContext());
            _jobsDbManager = new JobsDbManager(new BaseContext());
            _jobExecutor= JobExecutor.JobExecutor.Instance;
            var channelOptions = new List<ChannelOption>();
            channelOptions.Add(new ChannelOption(ChannelOptions.MaxReceiveMessageLength, -1));
            _channel = new Channel("localhost",8001,ChannelCredentials.Insecure,channelOptions);
            _gateWay = new GateWay.GateWayClient(_channel);
            
            var loggerFactory = new LoggerFactory()
                .AddConsole()
                .AddDebug();
            this._logger = loggerFactory.CreateLogger<DataBaseServerGrpc>();
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
                _jobExecutor.SetJobStatus(guid,EnumJobStatus.Error);
                this._logger.LogError($"ErrorResponse {guid}");
                var jobInfo = _jobExecutor.GetJob(guid).GetJobInfo();
                jobInfo.Message = e.ToString();
                await _gateWay.PostJobInfoAsync(jobInfo);
            };
        } 
        
        public override async Task<JobInfo> SaveFileInfo(GRPCService.GRPCProto.FileInfo request, ServerCallContext context)
        {
            _logger.LogInformation($"Save File Info ({JsonConvert.SerializeObject(request)})");
            var guid = Guid.NewGuid();
            var jobInfo = new JobInfo
            {
                JobStatus = EnumJobStatus.Execute, 
                JobId = guid.ToString()
            };
            try
            {
                var job = new AddFileInfoJob(_fileInfosDbManager, DBO.FileInfo.FromGRPCFileInfo(request))
                {
                    Guid = guid
                };
                _jobExecutor.JobAsyncExecute(job, GetHandleJobOk(), GetHandleJobError());
            }
            catch (AddException e)
            {
                Console.WriteLine(e);
            }
            return jobInfo;
        }

        public override async Task<JobInfo> UpdateOrCreateJob(JobInfo request, ServerCallContext context)
        {
            _logger.LogInformation($"Update Or Create Job ({JsonConvert.SerializeObject(request)})");
            var guid = Guid.NewGuid();
            var jobInfo = new JobInfo
            {
                JobStatus = EnumJobStatus.Execute, 
                JobId = guid.ToString()
            };
            try
            {
                var job = new UpdateOrCreateJobInfoJob(_jobsDbManager, request)
                {
                    Guid = guid
                };
                _jobExecutor.JobAsyncExecute(job, GetHandleJobOk(), GetHandleJobError());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return jobInfo;
        }

        public override async Task<JobInfo> GetJobInfo(JobInfo request, ServerCallContext context)
        {
            _logger.LogInformation($"Get Job Info ({JsonConvert.SerializeObject(request)})");
            var guid = Guid.NewGuid();
            var jobInfo = new JobInfo
            {
                JobStatus = EnumJobStatus.Execute, 
                JobId = guid.ToString()
            };
            try
            {
                var job = new GetJobInfoJob(_jobsDbManager, request.JobId)
                {
                    Guid = guid
                };
                _jobExecutor.JobAsyncExecute(job, GetHandleJobOk(), GetHandleJobError());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return jobInfo;
        }
        
        public override async Task<JobInfo> GetImagesInfo(ImagesInfo request, ServerCallContext context)
        {
            _logger.LogInformation($"Get Images Info ({JsonConvert.SerializeObject(request)})");
            var guid = Guid.NewGuid();
            var jobInfo = new JobInfo
            {
                JobStatus = EnumJobStatus.Execute, 
                JobId = guid.ToString()
            };
            try
            {
                var job = new GetImagesInfo(_jobsDbManager, request.JobId, request.FirstPageNo, request.Count)
                {
                    Guid = guid
                };
                _jobExecutor.JobAsyncExecute(job, GetHandleJobOk(), GetHandleJobError());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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

        public override async Task<JobInfo> DeleteJobInfo(JobInfo request, ServerCallContext context)
        {
            _logger.LogInformation($"Delete Job Info ({JsonConvert.SerializeObject(request)})");
            var guid = Guid.NewGuid();
            var jobInfo = new JobInfo
            {
                JobStatus = EnumJobStatus.Execute, 
                JobId = guid.ToString()
            };
            try
            {
                var job = new DeleteJobInfoJob(_jobsDbManager, request.JobId)
                {
                    Guid = guid
                };
                _jobExecutor.JobAsyncExecute(job, GetHandleJobOk(), GetHandleJobError());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return jobInfo;
        }

        public override async Task<JobInfo> DeleteFileInfo(FileInfo request, ServerCallContext context)
        {
            _logger.LogInformation($"Delete File Info ({JsonConvert.SerializeObject(request)})");
            var guid = Guid.NewGuid();
            var jobInfo = new JobInfo
            {
                JobStatus = EnumJobStatus.Execute, 
                JobId = guid.ToString()
            };
            try
            {
                var job = new DeleteFileInfo(_fileInfosDbManager, DBO.FileInfo.FromGRPCFileInfo(request))
                {
                    Guid = guid
                };
                _jobExecutor.JobAsyncExecute(job, GetHandleJobOk(), GetHandleJobError());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return jobInfo;
        }

        public override async Task<JobInfo> GetAllJobInfos(Empty request, ServerCallContext context)
        {
            _logger.LogInformation($"Get All Job Infos ({JsonConvert.SerializeObject(request)})");
            var guid = Guid.NewGuid();
            var jobInfo = new JobInfo
            {
                JobStatus = EnumJobStatus.Execute, 
                JobId = guid.ToString()
            };
            try
            {
                var job = new GetAllJobInfo(_jobsDbManager)
                {
                    Guid = guid
                };
                _jobExecutor.JobAsyncExecute(job, GetHandleJobOk(), GetHandleJobError());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return jobInfo;
        }

        public async void Dispose()
        {
            await _channel.ShutdownAsync();
        }
    }
}