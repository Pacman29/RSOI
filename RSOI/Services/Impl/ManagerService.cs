using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using JobExecutor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Models.Requests;
using Models.Responses;
using RSOI.Controllers;
using RSOI.Jobs;

namespace RSOI.Services.Impl
{
    public class ManagerService : IManagerService
    {
        private readonly IGateWayJobsFabric _gateWayJobsFabric;
        private readonly IJobExecutor _jobExecutor;
        private readonly ILogger<ManagerService> _logger;

        public ManagerService(IGateWayJobsFabric gateWayJobsFabric, 
            IJobExecutor jobExecutor,
            ILogger<ManagerService> logger)
        {
            _gateWayJobsFabric = gateWayJobsFabric;
            _jobExecutor = jobExecutor;
            this._logger = logger;
        }

        public async Task<IActionResult> RecognizePdf(PdfFile pdfFileModel)
        {
            _logger.LogInformation($"Recognize pdf request: {pdfFileModel.ToJson()}");
            var tcs = new TaskCompletionSource<IActionResult>();
            var task = tcs.Task;
            
            var recognizePdfJob = this._gateWayJobsFabric.GetRecognizePdfHighOrderJob(pdfFileModel);
            
            recognizePdfJob.OnHaveResult += async (jobInfo) =>
            {
                _logger.LogInformation($"Recognize pdf result: {jobInfo.ToJson()}");
                tcs.SetResult(new JsonResult(jobInfo));
            };
            
            this._jobExecutor.JobAsyncExecute(recognizePdfJob);
           
            
            return task.Result;
        }

        public async Task<IActionResult> GetJobStatus(string jobId)
        {
            _logger.LogInformation($"Job status request: {jobId}");
            var tcs = new TaskCompletionSource<IActionResult>();
            var task = tcs.Task;

            var getJobInfo = this._gateWayJobsFabric.GetJobStatusHighOrderJob(jobId);
            
            getJobInfo.OnHaveResult += async (jobInfo) =>
            {
                if (jobInfo != null)
                {
                    _logger.LogInformation($"Job status result: {jobInfo.ToJson()}");
                    tcs.SetResult(new JsonResult(jobInfo));
                }
                else
                {
                    var jobError = new JobError()
                    {
                        Message = "jobId is incorrect",
                        StatusCode = 400
                    };
                    _logger.LogWarning($"Job status error result: {jobError.ToJson()}");
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
            _logger.LogInformation($"Get pdf request: {jobId}");
            var tcs = new TaskCompletionSource<IActionResult>();
            var task = tcs.Task;

            var getPdfFileJob = _gateWayJobsFabric.GetPdfFileHighOrderJob(jobId);
            getPdfFileJob.OnHaveResult += async bytesPdf =>
            {
                _logger.LogInformation($"Get pdf result: {jobId}");
                tcs.SetResult(new FileContentResult(bytesPdf,"application/pdf"));
            };
            getPdfFileJob.OnHaveError += async jobError =>
            {
                _logger.LogWarning($"Get pdf {jobId} error result: {jobError.ToJson()}");
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
            _logger.LogInformation($"Get images request: {jobId}");
            var tcs = new TaskCompletionSource<IActionResult>();
            var task = tcs.Task;

            var getImagesJob = _gateWayJobsFabric.GetImagesHighOrderJob(jobId, firstPage, count);
            getImagesJob.OnHaveResult += async bytesZip =>
            {
                _logger.LogInformation($"Get images result: {jobId}");
                tcs.SetResult(new FileContentResult(bytesZip,"application/zip")); 
            };
            getImagesJob.OnHaveError += async jobError =>
            {
                _logger.LogWarning($"Get images {jobId} (params: firstPage: {firstPage} count: {count}) error result: {jobError.ToJson()}");
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
            _logger.LogInformation($"Get image request: {jobId}");
            var tcs = new TaskCompletionSource<IActionResult>();
            var task = tcs.Task;

            var getImageJob = _gateWayJobsFabric.GetImageHighOrderJob(jobId, pageNo);
            getImageJob.OnHaveResult += async bytesImg =>
            {
                _logger.LogInformation($"Get image result: {jobId}");
                tcs.SetResult(new FileContentResult(bytesImg,"image/jpeg"));
            };
            getImageJob.OnHaveError += async jobError =>
            {
                _logger.LogWarning($"Get image {jobId} (params : pageNo : {pageNo}) error result: {jobError.ToJson()}");
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
            _logger.LogInformation($"Delete job request: {jobId}");
            var tcs = new TaskCompletionSource<IActionResult>();
            var task = tcs.Task;

            var deleteJob = _gateWayJobsFabric.DeleteJobHighOrderJob(jobId);
            deleteJob.OnHaveResult += async res =>
            {
                _logger.LogInformation($"Delete job result: {jobId}");
                tcs.SetResult(new OkResult());
            };
            deleteJob.OnHaveError += async jobError =>
            {
                _logger.LogWarning($"Delete job {jobId} error result: {jobError.ToJson()}");
                tcs.SetResult(new ObjectResult(jobError)
                {
                    StatusCode = jobError.StatusCode
                });
            };
            
            this._jobExecutor.JobAsyncExecute(deleteJob);
            
            return task.Result;
        }

        public async Task<IActionResult> UpdateJob(string jobId, PdfFile pdfFileModel)
        {
            _logger.LogInformation($"Update job request: {jobId}");
            var tcs = new TaskCompletionSource<IActionResult>();
            var task = tcs.Task;

            var updateJob = _gateWayJobsFabric.UpdateJobHighOrderJob(jobId,pdfFileModel);
            updateJob.OnHaveResult += async res =>
            {
                _logger.LogInformation($"Update job result: {res.ToJson()}");
                tcs.SetResult(new JsonResult(res));
            };
            updateJob.OnHaveError += async jobError =>
            {
                _logger.LogWarning($"Update job {jobId} (params : pdfFile : {pdfFileModel.ToJson()}) error result: {jobError.ToJson()}");
                tcs.SetResult(new ObjectResult(jobError)
                {
                    StatusCode = jobError.StatusCode
                });
            };
            
            this._jobExecutor.JobAsyncExecute(updateJob);
            
            return task.Result;
        }

        public async Task<IActionResult> GetAllJobStatus()
        {
            _logger.LogInformation($"Get all jobs request");
            var tcs = new TaskCompletionSource<IActionResult>();
            var task = tcs.Task;

            var getAll = _gateWayJobsFabric.GetAllJobInfosHighOrderJob();
            getAll.OnHaveResult += async res =>
            {
                _logger.LogInformation($"Get all jobs result");
                tcs.SetResult(new JsonResult(res));
            };
            getAll.OnHaveError += async jobError =>
            {
                _logger.LogWarning($"Get all jobs error result: {jobError.ToJson()}");
                tcs.SetResult(new ObjectResult(jobError)
                {
                    StatusCode = jobError.StatusCode
                });
            };
            
            this._jobExecutor.JobAsyncExecute(getAll);
            
            return task.Result;
        }
    }
}