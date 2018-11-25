using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthOptions;
using AuthServer.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Requests;
using Models.Responses;

namespace AuthServer.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        

        public AccountController(
            IJwtTokenGenerator jwtTokenGenerator,
            UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager, 
            RoleManager<IdentityRole> roleManager
            )
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
        }
        
        [HttpPost, AllowAnonymous]
        public async Task<ActionResult<AccountResponseModel>> Index([FromBody] AccountRequestModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    if (user != null)
                    {
                        var jwt = _jwtTokenGenerator.GenerateJwtToken(user.Id,user.UserName);
                        return new AccountResponseModel()
                        {
                            Token = new JwtSecurityTokenHandler().WriteToken(jwt),
                            UserName = user.UserName
                        };
                    }
                }
            }
            return BadRequest(ModelState);
        }
        
        [HttpPost, Route("Register"), AllowAnonymous]
        public async Task<ActionResult<AccountResponseModel>> Register([FromBody] AccountRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new IdentityUser { UserName = model.UserName};

            var result = await _userManager.CreateAsync(user, model.Password);

            string role = Roles.RoleEnum.User.ToString();

            if (result.Succeeded)
            {
                if (await _roleManager.FindByNameAsync(role) == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
                await _userManager.AddToRoleAsync(user, role);
                await _userManager.AddClaimAsync(user, new Claim("userName", user.UserName));
                await _userManager.AddClaimAsync(user, new Claim("role", role));

                return Ok(user.UserName);
            }

            return BadRequest(result.Errors);
        }
        
        [HttpGet, Route("RefreshToken")]
        public async Task<ActionResult<AccountResponseModel>> RefreshToken()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByIdAsync(GetJwtUserId);
                if (user != null)
                {
                    var jwt = _jwtTokenGenerator.GenerateJwtToken(user.Id,user.UserName);
                    return new AccountResponseModel()
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(jwt),
                        UserName = user.UserName
                    };
                }
            }
            return BadRequest();
        }
        
        [HttpPost, Route("PasswordChange")] 
        public async Task<IActionResult> PasswordChange([FromBody] PasswordUpdateRequestModel updateModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (this.User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByIdAsync(GetJwtUserId);
                if (user != null)
                {
                    var result = await _userManager.ChangePasswordAsync(user, updateModel.OldPassword, updateModel.Password);

                    if (result.Succeeded)
                        return Ok();
                }
            }
            return BadRequest();
        }
        
        private string GetJwtUserId => User.GetJwtUserId();
        
    } 
}