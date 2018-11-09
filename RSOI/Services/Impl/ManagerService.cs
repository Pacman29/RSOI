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
            var tcs = new TaskCompletionSource<string>();
            var task = tcs.Task;
            
            var recognizePdfJob = this._gateWayJobsFabric.GetRecognizePdfHighOrderJob(pdfFileModel);
            
            recognizePdfJob.OnHaveResult += (job) =>
            {
                string jobId;
                var bf = new BinaryFormatter();
                using (var ms = new MemoryStream(recognizePdfJob.Bytes))
                    jobId = (string) bf.Deserialize(ms);
                tcs.SetResult(jobId);
            };
            
            this._jobExecutor.JobAsyncExecute(recognizePdfJob);
           
            
            return new JsonResult(new JobInfo()
            {
                JobId = task.Result
            });
        }

        public async Task<IActionResult> GetJobStatus(string JobId)
        {
            return new JsonResult(new JobInfo()
            {
                JobId = JobId,
                
            });
        }
    }
}