using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers
{
    public class AccountController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return
            View();
        }
    }
}