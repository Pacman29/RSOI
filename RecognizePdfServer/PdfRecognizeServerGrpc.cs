using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBaseServer.Services;
using Grpc.Core;
using GRPCService.GRPCProto;
using JobExecutor;
using RecognizePdfServer.Jobs;

namespace RecognizePdfServer
{
    public class PdfRecognizeServerGrpc : Recognize.RecognizeBase, IDisposable
    {
        private readonly IJobExecutor _jobExecutor;
        private readonly GateWay.GateWayClient _gateWay;

        public PdfRecognizeServerGrpc()
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
        
        public override async Task<Empty> RecognizePdf(PdfFile request, ServerCallContext context)
        {
            var memoryStream = new MemoryStream(request.Bytes.ToByteArray());
            var pages = request.Pages.ToArray();
            var jobId = request.JobId;
                
            try
            {
                var job = new RecognizePdfJob(memoryStream,pages)
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

        public void Dispose()
        {
        }
    }
}