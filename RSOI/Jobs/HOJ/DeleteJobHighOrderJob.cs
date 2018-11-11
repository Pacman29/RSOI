using System.Threading.Tasks;
using GRPCService.GRPCProto;

namespace RSOI.Jobs
{
    public class DeleteJobHighOrderJob : GateWayJob<byte[]>
    {
        private readonly string _jobId;
        public IGateWayJobsFabric GateWayJobsFabric { get; set; }
        
        public DeleteJobHighOrderJob(string jobId)
        {
            _jobId = jobId;
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
                        var updateJob = GateWayJobsFabric.GetUpdateJobToDatabase(this._jobId, EnumJobStatus.Execute);
                        updateJob.RunNextOnHaveResult(async (jobId) =>
                        {
                            var deleteJobFromDatabase = GateWayJobsFabric.DeleteJobFromDatabase(this._jobId);
                            deleteJobFromDatabase.OnHaveResult += async (res) =>
                            {
                                if (res)
                                {
                                    var packageJob = GateWayJobsFabric.GetPackageJob();
                                    packageJob.OnDone += async job =>
                                    {
                                        this.InvokeOnHaveResult(null);
                                    };
                                    var deletePdf = GateWayJobsFabric.DeleteFileJob(jobInfo.PdfPath);
                                    jobInfo.Images.ForEach(img =>
                                    {
                                        var deleteImgJob = GateWayJobsFabric.DeleteFileJob(img.Path);
                                        packageJob.AddJob(deleteImgJob);
                                    });
                                    
                                    packageJob.AddJob(deletePdf);
                                    this.Executor.JobAsyncExecute(packageJob);
                                }
                                else
                                {
                                    this.InvokeOnHaveError(new JobError()
                                    {
                                        Message = "Job not delete",
                                        StatusCode = 500
                                    });
                                }
                            };

                            return deleteJobFromDatabase;
                        });
                        this.Executor.JobAsyncExecute(updateJob);
                    } 
                }
                    
            };
            this.Executor.JobAsyncExecute(getJobsStatusHOJ);
        }
    }
}