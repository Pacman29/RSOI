using System;
using System.Threading.Tasks;
using GRPCService.GRPCProto;
using JobExecutor;
using RSOI.Services;
using RSOI.Services.Impl;

namespace RSOI.Jobs
{
    public class DeleteFileJob : GateWayJob<bool>
    {
        private readonly string _path;
        public IFileService FileService { get; set; }


        public DeleteFileJob(string path)
        {
            _path = path;
            this.OnDone += async (job) =>
            {
                this.InvokeOnHaveResult(BytesDeserializer<bool>.Deserialize(job));
            };
        }

        public override async Task ExecuteAsync()
        {
            var jobInfo = await FileService.DeleteFile(this._path);
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }
    }
}