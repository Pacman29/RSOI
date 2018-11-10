using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using RSOI.Services;

namespace RSOI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IManagerService _managerService;

        public FilesController(IManagerService managerService)
        {
            _managerService = managerService;
        }
        
        // GET
        public IActionResult Index()
        {
            return Ok("get Files");
        }

        [HttpGet]
        [Route("{jobId}/pdf")]
        public async Task<IActionResult> GetPdf(string jobId)
        {
            return await _managerService.GetPdf(jobId);
        }

        [HttpGet]
        [Route("{jobId}/images")]
        public async Task<IActionResult> GetImages(string jobId,[FromQuery] ImagesRequestModel model)
        {
            return await _managerService.GetImages(jobId, model.FirstPage, model.Count);
        }
        
        [HttpGet]
        [Route("{jobId}/image")]
        public async Task<IActionResult> GetImage(string jobId,[FromQuery] ImageRequestModel model)
        {
            return await _managerService.GetImage(jobId, model.PageNo);
        }
    }
}