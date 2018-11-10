using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using JobExecutor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
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
            var tcs = new TaskCompletionSource<IActionResult>();
            var task = tcs.Task;
            
            var recognizePdfJob = this._gateWayJobsFabric.GetRecognizePdfHighOrderJob(pdfFileModel);
            
            recognizePdfJob.OnHaveResult += async (jobInfo) =>
            {
                tcs.SetResult(new JsonResult(jobInfo));
            };
            
            this._jobExecutor.JobAsyncExecute(recognizePdfJob);
           
            
            return task.Result;
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
                    var jobError = new JobError()
                    {
                        Message = "jobId is incorrect",
                        StatusCode = 400
                    };
                    tcs.SetResult(new ObjectResult(jobError)
                    {
                        StatusCode = jobError.StatusCode
                    });
                }
                
            };
            
            this._jobExecutor.JobAsyncExecute(getJobInfo);
            return task.Result;
        }

        public async Task<IActionResult> GetPdf(string jobId)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var task = tcs.Task;

            var getPdfFileJob = _gateWayJobsFabric.GetPdfFileHighOrderJob(jobId);
            getPdfFileJob.OnHaveResult += async bytesPdf =>
            {
                tcs.SetResult(new FileContentResult(bytesPdf,"application/pdf"));
            };
            getPdfFileJob.OnHaveError += async jobError =>
            {
                tcs.SetResult(new ObjectResult(jobError)
                {
                    StatusCode = jobError.StatusCode
                });
            };
            this._jobExecutor.JobAsyncExecute(getPdfFileJob);
            return task.Result;
        }

        public async Task<IActionResult> GetImages(string jobId, int firstPage, int count)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var task = tcs.Task;

            var getImagesJob = _gateWayJobsFabric.GetImagesHighOrderJob(jobId, firstPage, count);
            getImagesJob.OnHaveResult += async bytesZip =>
            {
                tcs.SetResult(new FileContentResult(bytesZip,"application/zip")); 
            };
            getImagesJob.OnHaveError += async jobError =>
            {
                tcs.SetResult(new ObjectResult(jobError)
                {
                    StatusCode = jobError.StatusCode
                });
            };
            
            this._jobExecutor.JobAsyncExecute(getImagesJob);
            return task.Result;
        }
        
        public async Task<IActionResult> GetImage(string jobId, long pageNo)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var task = tcs.Task;

            var getImageJob = _gateWayJobsFabric.GetImageHighOrderJob(jobId, pageNo);
            getImageJob.OnHaveResult += async bytesImg =>
            {
                tcs.SetResult(new FileContentResult(bytesImg,"image/jpeg"));
            };
            getImageJob.OnHaveError += async jobError =>
            {
                tcs.SetResult(new ObjectResult(jobError)
                {
                    StatusCode = jobError.StatusCode
                });
            };
            
            this._jobExecutor.JobAsyncExecute(getImageJob);
            return task.Result;
        }

        public async Task<IActionResult> DeleteJob(string jobId)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var task = tcs.Task;

            
            
            return task.Result;
        }

        public async Task<IActionResult> UpdateJob(string jobId, PdfFile pdfFileModel)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var task = tcs.Task;


            return task.Result;
        }
    }
}