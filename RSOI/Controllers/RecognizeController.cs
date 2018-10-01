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
        private readonly IRecognizeService recognizeService;
        
        public RecognizeController(IRecognizeService _recognizeService)
        {
            recognizeService = _recognizeService;
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