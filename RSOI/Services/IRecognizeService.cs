using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;

namespace RSOI.Services
{
    public interface IRecognizeService
    {
        Task<IActionResult> RecognizePdf(PdfFile pdfFile);
    }
}