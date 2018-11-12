using System.Threading.Tasks;
using GRPCService.GRPCProto;

namespace RSOI.Jobs
{
    public class GetImageHighOrderJob : GateWayJob<byte[]>
    {
        private readonly string _jobId;
        private readonly long _pageNo;

        public IGateWayJobsFabric GateWayJobsFabric { get; set; }

        public GetImageHighOrderJob(string jobId, long pageNo)
        {
            _jobId = jobId;
            _pageNo = pageNo;
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
                       var imageInfo = jobInfo.Images.Find(img => img.PageNo == this._pageNo);
                       if (imageInfo != null)
                       {
                           var getImgFileJob = GateWayJobsFabric.GetFileJob(imageInfo.Path);
                           getImgFileJob.OnHaveResult += async img =>
                           {
                               this.InvokeOnHaveResult(img);
                           };
                           this.Executor.JobAsyncExecute(getImgFileJob);
                       }
                       else
                       {
                           this.InvokeOnHaveError(new JobError()
                           {
                               Message = "PageNo is incorrect",
                               StatusCode = 400
                           });
                       }
                   } 
                }
                    
            };
            this.Executor.JobAsyncExecute(getJobsStatusHOJ);
        }
    }
}