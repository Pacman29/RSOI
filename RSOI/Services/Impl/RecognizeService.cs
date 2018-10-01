using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;

namespace RSOI.Services.Impl
{
    public class RecognizeService : IRecognizeService
    {
        private readonly IDataBaseService dataBaseService;
        
        public RecognizeService(IDataBaseService dataBaseService)
        {
            this.dataBaseService = dataBaseService;
        }

        public async Task<IActionResult> RecognizePdf(PdfFile pdfFile)
        {
            IActionResult result = null;
            try
            {
                dataBaseService.CreatePdfFile(await pdfFile.GetPdfFileInfo());
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