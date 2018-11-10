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
                if (jobInfo == null)
                {
                    this.InvokeOnHaveError(new JobError()
                    {
                        Message = "id is incorrect",
                        StatusCode = 400
                    });
                }
                else
                {
                    if (jobInfo.JobStatus != "Done")
                    {
                        this.InvokeOnHaveError(new JobError()
                        {
                            Message = "Job execute",
                            StatusCode = 409
                        });
                    }
                    else
                    {
                        var getImagesInfoJob =
                            GateWayJobsFabric.GetImagesInfoJob(this._jobId, this._pageNo, this._count);
                        getImagesInfoJob.RunNextOnHaveResult(async images =>
                        {
                            var getFilesJob =
                                GateWayJobsFabric.GetFilesJob(images.Images.Select(img => img.Path).ToList());
                            getFilesJob.OnHaveResult += async zip => { this.InvokeOnHaveResult(zip); };
                            return getFilesJob;
                        });
                        this.Executor.JobAsyncExecute(getImagesInfoJob);
                    }
                }
            };
            this.Executor.JobAsyncExecute(getJobsStatusHOJ);
        }
    }
}