using System;
using System.Threading.Tasks;
using DataBaseServer.Contexts;
using DataBaseServer.DBO;
using DataBaseServer.Exceptions.DBExceptions;
using DataBaseServer.Jobs;
using Grpc.Core;
using GRPCService.GRPCProto;

namespace DataBaseServer
{
    public class DataBaseServerGrpc : DataBase.DataBaseBase, IDisposable
    {
        private readonly FileInfosDbManager _fileInfosDbManager;
        private readonly JobExecutor.IJobExecutor _jobExecutor;
        private readonly GateWay.GateWayClient _gateWay;
        private readonly Channel _channel;
        private readonly JobsDbManager _jobsDbManager;

        public DataBaseServerGrpc() : base()
        {
            var context = new BaseContext();
            _fileInfosDbManager = new FileInfosDbManager(context);
            _jobsDbManager = new JobsDbManager(context);
            _jobExecutor= JobExecutor.JobExecutor.Instance;
            _channel = new Channel("localhost",8001,ChannelCredentials.Insecure);
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
                _jobExecutor.SetJobStatus(guid,EnumJobStatus.Error);
                var jobInfo = _jobExecutor.GetJob(guid).GetJobInfo();
                jobInfo.Message = e.ToString();
                await _gateWay.PostJobInfoAsync(jobInfo);
            };
        } 
        
        public override async Task<JobInfo> SavePdfFileInfo(PdfFileInfo request, ServerCallContext context)
        {
            var guid = Guid.NewGuid();
            var jobInfo = new JobInfo
            {
                JobStatus = EnumJobStatus.Execute, 
                JobId = guid.ToString()
            };
            try
            {
                var job = new AddPdfFileJob(_fileInfosDbManager, FileInfo.FromPdfFileInfo(request)) {Guid = guid};
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
            var guid = Guid.NewGuid();
            var jobInfo = new JobInfo
            {
                JobStatus = EnumJobStatus.Execute, 
                JobId = guid.ToString()
            };
            try
            {
                var job = new UpdateOrCreateJobInfoJob(_jobsDbManager,request);
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

        public async void Dispose()
        {
            await _channel.ShutdownAsync();
        }
    }
}