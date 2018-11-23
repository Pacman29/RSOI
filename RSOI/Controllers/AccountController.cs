using System.Threading.Tasks;
using AuthOptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Responses;
using RSOI.Services;

namespace RSOI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller, IAuthController
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }
        
        
        [HttpPost, AllowAnonymous]
        public async Task<ActionResult<AccountResponseModel>> Index([FromBody] AccountRequestModel model)
        {
            if (ModelState.IsValid)
                return await _authService.Login(model);

            return BadRequest(ModelState.ToString());
        }
        
        [HttpPost, Route("Register"), AllowAnonymous]
        public async Task<ActionResult<AccountResponseModel>> Register([FromBody] AccountRequestModel model)
        {
            if (ModelState.IsValid)
                return await _authService.Register(model);

            return BadRequest(ModelState.ToString());
        }
        
        [HttpGet, Route("RefreshToken")]
        public async Task<ActionResult<AccountResponseModel>> RefreshToken()
        {
            if (ModelState.IsValid)
                return await _authService.RefreshToken();

            return BadRequest(ModelState.ToString());
        }
        
        [HttpPost, Route("PasswordChange")] 
        public async Task<IActionResult> PasswordChange([FromBody] PasswordUpdateRequestModel updateModel)
        {
            if (ModelState.IsValid)
                return await _authService.PasswordChange(updateModel);

            return BadRequest(ModelState.ToString());
        }
    }
}