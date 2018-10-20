using System;
using System.Threading.Tasks;
using DataBaseServer.Contexts;
using DataBaseServer.DBO;
using DataBaseServer.Exceptions.DBExceptions;
using DataBaseServer.Jobs;
using DataBaseServer.Services;
using Grpc.Core;
using GRPCService.GRPCProto;
using JobExecutor;
using Microsoft.EntityFrameworkCore;

namespace DataBaseServer
{
    public class DataBaseServerGrpc : DataBase.DataBaseBase, IDisposable
    {
        private readonly FileInfosContext _fileInfosContext;
        private readonly JobExecutor.IJobExecutor _jobExecutor;
        private readonly IGateWayService _gateWayService;
        
        public DataBaseServerGrpc() : base()
        {
            _fileInfosContext = new FileInfosContext();
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
        
        public override async Task<Empty> SavePdfFileInfo(PdfFileInfo request, ServerCallContext context)
        {
            var result = new Empty();
            try
            {
                var job = new AddPdfFileJob(_fileInfosContext, FileInfo.fromPdfFileInfo(request))
                {
                    Guid = new Guid(request.JobId)
                };
                var jobInfo = new JobInfo
                {
                    JobStatus = EnumJobStatus.Execute, 
                    JobId = request.JobId
                };
                _jobExecutor.AddJob(job, GetHandleJobOk(), GetHandleJobError());
                await _gateWayService.SendJobInfo(jobInfo);
            }
            catch (AddException e)
            {
                Console.WriteLine(e);
            }


            return result;
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
            _fileInfosContext?.Dispose();
        }
    }
}