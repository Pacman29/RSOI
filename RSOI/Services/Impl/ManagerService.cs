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
            var recognizePdfJob = this._gateWayJobsFabric.GetRecognizePdfJob(pdfFileModel);
            this._jobExecutor.JobAsyncExecute(recognizePdfJob);

            while (recognizePdfJob.Bytes == null)
                await Task.Delay(TimeSpan.FromSeconds(10));
            
            
            string jobId;
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream(recognizePdfJob.Bytes))
                jobId = (string) bf.Deserialize(ms);
            
            return new JsonResult(new JobInfo()
            {
                JobId = jobId
            });
        }
    }
}