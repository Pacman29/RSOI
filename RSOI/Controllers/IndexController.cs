
using Microsoft.AspNetCore.Mvc;

namespace RSOI.Controllers
{
    [Route("/")]
    public class IndexController : ControllerBase
    {
        public IActionResult Index()
        {
            return new OkResult();
        }
    }
}