using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Responses;

namespace AuthOptions
{
    public interface IAuthController
    {
        Task<ActionResult<AccountResponseModel>> Index([FromBody] AccountRequestModel model);
        Task<ActionResult<AccountResponseModel>> Register([FromBody] AccountRequestModel model);
        Task<ActionResult<AccountResponseModel>> RefreshToken();
        Task<IActionResult> PasswordChange([FromBody] PasswordUpdateRequestModel updateModel);
    }
}