using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;

namespace RSOI.Services
{
    public interface IManagerService
    {
        Task<IActionResult> RecognizePdf(PdfFile pdfFileModel);
        Task<IActionResult> GetAllJobStatus();
        Task<IActionResult> GetJobStatus(string jobId);
        Task<IActionResult> GetPdf(string jobId);
        Task<IActionResult> GetImages(string jobId, int firstPage, int count);
        Task<IActionResult> GetImage(string jobId, long pageNo);
        Task<IActionResult> DeleteJob(string jobId);
        Task<IActionResult> UpdateJob(string jobId, PdfFile pdfFileModel);
    }
}