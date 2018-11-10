using System.Threading.Tasks;
using RSOI.Services;

namespace RSOI.Jobs
{
    public class GetPdfFileHighOrderJob : GateWayJob<byte[]>
    {
        private string _jobId;
        public IGateWayJobsFabric GateWayJobsFabric { get; set; }

        public GetPdfFileHighOrderJob(string jobId)
        {
            this._jobId = jobId;
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
                    var path = jobInfo.PdfPath;
                    var getPdfFileJob = GateWayJobsFabric.GetFileJob(path);
                    getPdfFileJob.OnHaveResult += async bytePdf =>
                    {
                        this.InvokeOnHaveResult(bytePdf);  
                    };
                    
                    this.Executor.JobAsyncExecute(getPdfFileJob);
                }
            };
            this.Executor.JobAsyncExecute(getJobsStatusHOJ);
        }
    }
}