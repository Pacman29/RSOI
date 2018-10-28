using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DataBaseServer.Services;
using Grpc.Core;
using GRPCService.GRPCProto;
using JobExecutor;
using RecognizePdfServer.Jobs;

namespace RecognizePdfServer
{
    public class PdfRecognizeServerGrpc : RecognizeService.RecognizeServiceBase, IDisposable
    {
        private IJobExecutor _jobExecutor;
        private GateWayService _gateWayService;

        public PdfRecognizeServerGrpc()
        {
            _jobExecutor= JobExecutor.JobExecutor.Instance;
            _gateWayService = new GateWayService("localhost:8001");
        }

        private Action<Guid> GetHandleJobOk()
        {
            return async (Guid guid) =>
            {
                _jobExecutor.SetJobStatus(guid,EnumJobStatus.Done);
                var jobInfo = _jobExecutor.GetJob(guid).GetJobInfo();
                await _gateWayService.SendJobInfo(jobInfo);
            };
        }

        private Action<Guid, Exception> GetHandleJobError()
        {
            return async (Guid guid, Exception e) =>
            {
                _jobExecutor.SetJobStatus(guid,EnumJobStatus.Error);
                var jobInfo = _jobExecutor.GetJob(guid).GetJobInfo();
                jobInfo.Message = e.ToString();
                await _gateWayService.SendJobInfo(jobInfo);
            };
        }
        
        public override async Task<Empty> RecognizePdf(IAsyncStreamReader<PdfFile> requestStream, ServerCallContext context)
        {
            var memoryStream = new MemoryStream(requestStream.Current.Filelength);
            var cancelToken = new CancellationToken();
            var fileLength = 0;
            var fileName = "";
            var jobId = "";
            while (await requestStream.MoveNext(cancelToken))
            {
                requestStream.Current.Bytes.WriteTo(memoryStream);
                fileLength = requestStream.Current.Filelength;
                fileName = requestStream.Current.Filename;
                jobId = requestStream.Current.JobId;
            }
                
            try
            {
                var job = new RecognizePdfJob(memoryStream,fileLength,fileName)
                {
                    Guid = new Guid(jobId)
                };
                var jobInfo = new JobInfo
                {
                    JobStatus = EnumJobStatus.Execute, 
                    JobId = jobId
                };
                _jobExecutor.AddJob(job, GetHandleJobOk(), GetHandleJobError());
                await _gateWayService.SendJobInfo(jobInfo);
                
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