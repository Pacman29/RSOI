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
using File = GRPCService.GRPCProto.File;
using Path = GRPCService.GRPCProto.Path;

namespace FileServer
{
    public class FileServerGrpc : GRPCService.GRPCProto.FileServer.FileServerBase, IDisposable
    {
        private readonly IJobExecutor _jobExecutor;
        private readonly GateWay.GateWayClient _gateWay;
        private readonly Channel _channel;

        public FileServerGrpc()
        {
            _jobExecutor= JobExecutor.JobExecutor.Instance;
            var channelOptions = new List<ChannelOption>();
            channelOptions.Add(new ChannelOption(ChannelOptions.MaxReceiveMessageLength, -1));
            _channel = new Channel("localhost",8001,ChannelCredentials.Insecure,channelOptions);
            _gateWay = new GateWay.GateWayClient(_channel);
        }

        private Action<Guid> GetHandleJobOk()
        {
            return async (Guid guid) =>
            {
                _jobExecutor.SetJobStatus(guid,EnumJobStatus.Done);
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
                Console.WriteLine(e);
                _jobExecutor.SetJobStatus(guid,EnumJobStatus.Error);
                var jobInfo = _jobExecutor.GetJob(guid).GetJobInfo();
                jobInfo.Message = e.ToString();
                await _gateWay.PostJobInfoAsync(jobInfo);
            };
        }

        public override async Task<JobInfo> SaveFile(File request, ServerCallContext context)
        {
            Console.WriteLine("Save File");
            var memoryStream = new MemoryStream(request.Bytes.ToByteArray());
            var guid = Guid.NewGuid();
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
            Console.WriteLine("Get File");
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
            Console.WriteLine("Get Files");
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

        public override Task<JobInfo> DeleteFile(Path request, ServerCallContext context)
        {
            return base.DeleteFile(request, context);
        }

        public async void Dispose()
        {
           await _channel.ShutdownAsync();
        }
    }
}