using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using RSOI.Services;

namespace RSOI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecognizeController : ControllerBase
    {
        private IManagerService _managerService;

        public RecognizeController(IManagerService managerService)
        {
            _managerService = managerService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Ok("get Recognize pdf");
        }

        [HttpPost]
        public async Task<IActionResult> PostPdf([FromForm] PdfFile request)
        {
            return await recognizeService.RecognizePdf(request);
        }
    }
}