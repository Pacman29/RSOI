using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;

namespace RSOI.Services.Impl
{
    public class RecognizeService : IRecognizeService
    {
        public async Task<IActionResult> RecognizePdf(PdfFile pdfFile)
        {
            throw new System.NotImplementedException();
        }
    }
}