using System;
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
        private readonly IRecognizeService _recognizeService;
        private readonly IDataBaseService _dataBaseService;
        private readonly IFileService _fileService;

        private readonly IJobExecutor _jobExecutor;
        
        public ManagerService(IRecognizeService recognizeService, 
            IFileService fileService, 
            IDataBaseService dataBaseService, 
            IJobExecutor jobExecutor)
        {
            _fileService = fileService;
            _recognizeService = recognizeService;
            _dataBaseService = dataBaseService;
            _jobExecutor = jobExecutor;
        }

        public async Task<IActionResult> RecognizePdf(PdfFile pdfFileModel)
        {
            var recognizePdfJob = new RecognizePdfRootJob(
                Guid.NewGuid(), 
                pdfFileModel, 
                _dataBaseService,
                _recognizeService, 
                _fileService);

            this._jobExecutor.JobAsyncExecute(recognizePdfJob);
            var spin = new SpinWait();

            while (recognizePdfJob.JobId == null)
                spin.SpinOnce();
            
            return new JsonResult(new JobInfo()
            {
                JobId = recognizePdfJob.JobId
            });
        }
    }
}