using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Models.Responses;

namespace RSOI.Jobs
{
    public class GetImagesHighOrderJob : GateWayJob<byte[]>
    {
        private readonly string _jobId;
        private readonly int _pageNo;
        private readonly int _count;
        public IGateWayJobsFabric GateWayJobsFabric { get; set; }
        
        public GetImagesHighOrderJob(string jobId, int pageNo, int count)
        {
            _jobId = jobId;
            _pageNo = pageNo;
            _count = count;
        }

        public override async Task ExecuteAsync()
        {
            var getJobsStatusHOJ = GateWayJobsFabric.GetJobStatusHighOrderJob(this._jobId);
            getJobsStatusHOJ.OnHaveResult += async (jobInfo) =>
            {
                if (jobInfo.JobStatus != "Done")
                {
                    this.InvokeOnHaveResult(null);
                }
                else
                {
                    var getImagesInfoJob = GateWayJobsFabric.GetImagesInfoJob(this._jobId, this._pageNo, this._count);
                    getImagesInfoJob.RunNextOnHaveResult(async images =>
                    {
                        var getFilesJob = GateWayJobsFabric.GetFilesJob(images.Images.Select(img => img.Path).ToList());
                        getFilesJob.OnHaveResult += async zip => { this.InvokeOnHaveResult(zip); };
                        return getFilesJob;
                    });
                    this.Executor.JobAsyncExecute(getImagesInfoJob);
                }
            };
            this.Executor.JobAsyncExecute(getJobsStatusHOJ);
        }
    }
}