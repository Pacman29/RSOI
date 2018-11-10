using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using JobExecutor;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Responses;
using RSOI.Jobs;

namespace RSOI.Services.Impl
{
    public class ManagerService : IManagerService
    {
        private readonly IGateWayJobsFabric _gateWayJobsFabric;
        private readonly IJobExecutor _jobExecutor;
        
        public ManagerService(IGateWayJobsFabric gateWayJobsFabric, 
            IJobExecutor jobExecutor)
        {
            _gateWayJobsFabric = gateWayJobsFabric;
            _jobExecutor = jobExecutor;
        }

        public async Task<IActionResult> RecognizePdf(PdfFile pdfFileModel)
        {
            var tcs = new TaskCompletionSource<JobInfo>();
            var task = tcs.Task;
            
            var recognizePdfJob = this._gateWayJobsFabric.GetRecognizePdfHighOrderJob(pdfFileModel);
            
            recognizePdfJob.OnHaveResult += async (jobInfo) =>
            {
                tcs.SetResult(jobInfo);
            };
            
            this._jobExecutor.JobAsyncExecute(recognizePdfJob);
           
            
            return new JsonResult(task.Result);
        }

        public async Task<IActionResult> GetJobStatus(string jobId)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var task = tcs.Task;

            var getJobInfo = this._gateWayJobsFabric.GetJobStatusHighOrderJob(jobId);
            
            getJobInfo.OnHaveResult += async (jobInfo) =>
            {
                if (jobInfo != null)
                {
                    tcs.SetResult(new JsonResult(jobInfo));
                }
                else
                {
                    tcs.SetResult(new NotFoundResult());
                }
                
            };
            
            this._jobExecutor.JobAsyncExecute(getJobInfo);
            return task.Result;
        }
    }
}