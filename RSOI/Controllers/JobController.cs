using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using RSOI.Services;

namespace RSOI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IManagerService _managerService;

        public JobController(IManagerService managerService)
        {
            _managerService = managerService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await _managerService.GetAllJobStatus();
        }

        [HttpPost]
        public async Task<IActionResult> PostPdf([FromForm] PdfFile request)
        {
            return await _managerService.RecognizePdf(request);
        }

        [HttpGet]
        [Route("{jobId}")]
        public async Task<IActionResult> GetJobStatus([FromRoute] string jobId)
        {
            return await _managerService.GetJobStatus(jobId);
        }
        
        [HttpDelete]
        [Route("{jobId}")]
        public async Task<IActionResult> DeleteJob([FromRoute] string jobId)
        {
            return await _managerService.DeleteJob(jobId);
        }
        
        [HttpPatch]
        [Route("{jobId}")]
        public async Task<IActionResult> UpdateJob([FromRoute] string jobId, [FromForm] PdfFile request)
        {
            return await _managerService.UpdateJob(jobId, request);
        }
    }
}