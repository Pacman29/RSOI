using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;
using JobExecutor;
using RSOI.Services;

namespace RSOI.Jobs
{
    public class GetFilesJob : GateWayJob<byte[]>
    {
        public IFileService FileService { get; set; }
        private readonly List<string> _paths;

        public GetFilesJob(List<string> paths)
        {
            _paths = paths;
            this.OnDone += async (job) =>
            {
                this.InvokeOnHaveResult(job.Bytes);
            };
        }

        public override async Task ExecuteAsync()
        {
            var jobInfo = await FileService.GetFiles(this._paths);
            this.ServiceGuid = new Guid(jobInfo.JobId);
            this.JobStatus = jobInfo.JobStatus;
        }
    }
}