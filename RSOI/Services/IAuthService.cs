using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Responses;

namespace RSOI.Services
{
    public interface IAuthService
    {
        Task<ActionResult<AccountResponseModel>> Login(AccountRequestModel model);
        Task<ActionResult<AccountResponseModel>> Register(AccountRequestModel model);
        Task<ActionResult<AccountResponseModel>> RefreshToken(string token);
        Task<IActionResult> PasswordChange(PasswordUpdateRequestModel updateModel, string token);
        
    }
}