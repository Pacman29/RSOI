using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using GRPCService.GRPCProto;
using JobExecutor;
using RSOI.Services;

namespace RSOI.Jobs
{
    public class AddFileInfoToDatabaseJob : GateWayJob<int>
    {
        private readonly GRPCService.GRPCProto.FileInfo _fileInfo;
        public IDataBaseService DataBaseService { get; set; }

        public AddFileInfoToDatabaseJob(GRPCService.GRPCProto.FileInfo fileInfo)
        {
            this._fileInfo = fileInfo;

            this.OnDone += async job =>
            {
                this.InvokeOnHaveResult(BytesDeserializer<int>.Deserialize(job));
            };
        }
        
        public override async Task ExecuteAsync()
        {
            var jobInfo = await DataBaseService.CreateFileInfo(this._fileInfo);
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }

        public override async Task Reject()
        {
            Console.WriteLine($"{this.Guid} job reject");
        }
    }
}