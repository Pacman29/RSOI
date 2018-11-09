using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;

namespace RSOI.Services
{
    public interface IManagerService
    {
        Task<IActionResult> RecognizePdf(PdfFile pdfFileModel);
        Task<IActionResult> GetJobStatus(string JobId);
    }
}