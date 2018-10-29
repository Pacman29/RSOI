using System;
using System.IO;
using System.Threading.Tasks;
using FileServer.Jobs;
using Grpc.Core;
using GRPCService.GRPCProto;
using JobExecutor;
using File = GRPCService.GRPCProto.File;
using Path = GRPCService.GRPCProto.Path;

namespace FileServer
{
    public class FileServerGrpc : GRPCService.GRPCProto.FileServer.FileServerBase
    {
        private readonly IJobExecutor _jobExecutor;
        private readonly GateWay.GateWayClient _gateWay;

        public FileServerGrpc()
        {
            _jobExecutor= JobExecutor.JobExecutor.Instance;
            var channel = new Channel("localhost",8001,ChannelCredentials.Insecure);
            _gateWay = new GateWay.GateWayClient(channel);
        }

        private Action<Guid> GetHandleJobOk()
        {
            return async (Guid guid) =>
            {
                _jobExecutor.SetJobStatus(guid,EnumJobStatus.Done);
                var jobInfoWithBytes = _jobExecutor.GetJob(guid).GetJobInfoWithBytes();
                await _gateWay.PostJobInfoWithBytesAsync(jobInfoWithBytes);
            };
        }

        private Action<Guid, Exception> GetHandleJobError()
        {
            return async (Guid guid, Exception e) =>
            {
                _jobExecutor.SetJobStatus(guid,EnumJobStatus.Error);
                var jobInfo = _jobExecutor.GetJob(guid).GetJobInfo();
                jobInfo.Message = e.ToString();
                await _gateWay.PostJobInfoAsync(jobInfo);
            };
        }
        
        public override async Task<Empty> SaveFile(File request, ServerCallContext context)
        {
            var memoryStream = new MemoryStream(request.Bytes.ToByteArray());
            var jobId = request.FilePath.JobId;
                
            try
            {
                var job = new SaveFileJob(request.FilePath.Path_, memoryStream)
                {
                    Guid = new Guid(jobId)
                };
                var jobInfo = new JobInfo
                {
                    JobStatus = EnumJobStatus.Execute, 
                    JobId = jobId
                };
                _jobExecutor.JobExecute(job, GetHandleJobOk(), GetHandleJobError());
                await _gateWay.PostJobInfoAsync(jobInfo);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            return new Empty();
        }

        public override async Task<Empty> GetFile(Path request, ServerCallContext context)
        {
            var jobId = request.JobId;
                
            try
            {
                var job = new GetFileJob(request.Path_)
                {
                    Guid = new Guid(jobId)
                };
                var jobInfo = new JobInfo
                {
                    JobStatus = EnumJobStatus.Execute, 
                    JobId = jobId
                };
                _jobExecutor.JobExecute(job, GetHandleJobOk(), GetHandleJobError());
                await _gateWay.PostJobInfoAsync(jobInfo);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            return new Empty();
        }

        public override Task<Empty> DeleteFile(Path request, ServerCallContext context)
        {
            return base.DeleteFile(request, context);
        }
    }
}