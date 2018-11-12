using System;
using System.Threading.Tasks;
using GRPCService.GRPCProto;
using RSOI.Services;

namespace RSOI.Jobs
{
    public class GetFileJob : GateWayJob<byte[]>
    {
        private readonly string _path;
        public IFileService FileService { get; set; }
        public GetFileJob(string path)
        {
            this._path = path;

            this.OnDone += async (job) =>
            {
                this.InvokeOnHaveResult(job.Bytes);
            };
        }

        public override async Task ExecuteAsync()
        {
            var jobInfo = await FileService.GetFile(new Path()
            {
                Path_ = this._path
            });
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }
    }
}