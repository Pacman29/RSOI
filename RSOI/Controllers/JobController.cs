using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Requests;
using RSOI.Services;

namespace RSOI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IManagerService _managerService;
        private readonly ILogger<FilesController> _logger;

        public JobController(IManagerService managerService, ILogger<FilesController> logger)
        {
            _managerService = managerService;
            this._logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation(this.Request.ToString());
            return await _managerService.GetAllJobStatus();
        }

        [HttpPost]
        public async Task<IActionResult> PostPdf([FromForm] PdfFile request)
        {
            _logger.LogInformation(this.Request.ToString());
            return await _managerService.RecognizePdf(request);
        }

        [HttpGet]
        [Route("{jobId}")]
        public async Task<IActionResult> GetJobStatus([FromRoute] string jobId)
        {
            _logger.LogInformation(this.Request.ToString());
            return await _managerService.GetJobStatus(jobId);
        }
        
        [HttpDelete]
        [Route("{jobId}")]
        public async Task<IActionResult> DeleteJob([FromRoute] string jobId)
        {
            _logger.LogInformation(this.Request.ToString());
            return await _managerService.DeleteJob(jobId);
        }
        
        [HttpPatch]
        [Route("{jobId}")]
        public async Task<IActionResult> UpdateJob([FromRoute] string jobId, [FromForm] PdfFile request)
        {
            _logger.LogInformation(this.Request.ToString());
            return await _managerService.UpdateJob(jobId, request);
        }
    }
}