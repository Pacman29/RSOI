using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Requests;
using RSOI.Services;

namespace RSOI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IManagerService _managerService;

        private readonly ILogger<FilesController> _logger;

        public FilesController(IManagerService managerService, ILogger<FilesController> logger)
        {
            _managerService = managerService;
            _logger = logger;
        }

        [HttpGet]
        [Route("{jobId}/pdf")]
        public async Task<IActionResult> GetPdf(string jobId)
        {
            _logger.LogInformation(this.Request.ToString());
            return await _managerService.GetPdf(jobId);
        }

        [HttpGet]
        [Route("{jobId}/images")]
        public async Task<IActionResult> GetImages(string jobId,[FromQuery] ImagesRequestModel model)
        {
            _logger.LogInformation(this.Request.ToString());
            return await _managerService.GetImages(jobId, model.FirstPage, model.Count);
        }
        
        [HttpGet]
        [Route("{jobId}/image")]
        public async Task<IActionResult> GetImage(string jobId,[FromQuery] ImageRequestModel model)
        {
            _logger.LogInformation(this.Request.ToString());
            return await _managerService.GetImage(jobId, model.PageNo);
        }
    }
}