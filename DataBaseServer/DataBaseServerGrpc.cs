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
            _gateWayService = new GateWayService("0.0.0.0:8001");
        }

        private Action<Guid> GetHandleJobOk()
        {
            return async (Guid guid) =>
            {
                var jobInfo = new JobInfo()
                {
                    JobId = guid.ToString(),
                    JobStatus = EnumJobStatus.Done
                };
                await _gateWayService.SendJobInfo(jobInfo);
            };
        }

        private Action<Guid, Exception> GetHandleJobError()
        {
            return async (Guid guid, Exception e) =>
            {
                var jobInfo = new JobInfo()
                {
                    JobId = guid.ToString(),
                    JobStatus = EnumJobStatus.Done,
                    Message = e.ToString()
                };
                await _gateWayService.SendJobInfo(jobInfo);
            };
        } 
        
        public override async Task<JobInfo> SavePdfFileInfo(PdfFileInfo request, ServerCallContext context)
        {
            var jobInfo = new JobInfo();
            try
            {
                var job = new AddPdfFileJob(_fileInfosContext, FileInfo.fromPdfFileInfo(request));
                jobInfo.JobId = job.GetJobId().ToString();
                _jobExecutor.AddJob(job, GetHandleJobOk(), GetHandleJobError());
                jobInfo.JobStatus = EnumJobStatus.Execute;
            }
            catch (AddException e)
            {
                Console.WriteLine(e);
            }


            return jobInfo;
        }

        public void Dispose()
        {
            _fileInfosContext?.Dispose();
        }
    }
}