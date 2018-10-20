using System;
using System.Threading.Tasks;
using JobExecutor;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using RSOI.Jobs;

namespace RSOI.Services.Impl
{
    public class RecognizeService : IRecognizeService
    {
        private readonly IDataBaseService dataBaseService;
        private IJobExecutor _jobExecutor;

        public RecognizeService(IDataBaseService dataBaseService)
        {
            this.dataBaseService = dataBaseService;
            this._jobExecutor = JobExecutor.JobExecutor.Instance;
        }

        public async Task<IActionResult> RecognizePdf(PdfFile pdfFile)
        {
            IActionResult result = null;
            try
            {
                var job = new AddPdfToDatabaseJob(
                    Guid.NewGuid(), 
                    dataBaseService, 
                    await pdfFile.GetPdfFileInfo()
                    );
                
                _jobExecutor.AddJob(job);
                
                result = new OkResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var error = new JsonResult(e);
                result = error;
            }

            return result;
        }
    }
}