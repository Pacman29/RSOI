
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RSOI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/")]
    public class IndexController : ControllerBase
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return new OkResult();
        }
    }
}