using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Responses;

namespace RSOI.Services.Impl
{
    public class AuthService : IAuthService
    {
        private readonly string _authServerUri;
        private readonly HttpClient _client;

        public AuthService(string authServerUri)
        {
            _authServerUri = authServerUri;
            _client = new HttpClient()
            {
                BaseAddress = new Uri($"http://{authServerUri}")
            };
        }
        
        public async Task<ActionResult<AccountResponseModel>> Login(AccountRequestModel model)
        {
            var response = await _client.PostAsJsonAsync("api/Account", model);
            if (response.IsSuccessStatusCode)
            {
                var data = AccountResponseModel.fromJson(await response.Content.ReadAsStringAsync());
                return new JsonResult(data);
            }
            return new JsonResult(await response.Content.ReadAsStringAsync())
            {
                StatusCode = (int)response.StatusCode,
            };
        }

        public async Task<ActionResult<AccountResponseModel>> Register(AccountRequestModel model)
        {
            var response = await _client.PostAsJsonAsync("api/Account/Register", model);
            if (response.IsSuccessStatusCode)
            {
                var data = AccountResponseModel.fromJson(await response.Content.ReadAsStringAsync());
                return new JsonResult(data);
            }
            return new JsonResult(await response.Content.ReadAsStringAsync())
            {
                StatusCode = (int)response.StatusCode,
            };
        }

        public async Task<ActionResult<AccountResponseModel>> RefreshToken()
        {
            var response = await _client.GetAsync("api/Account/RefreshToken");
            if (response.IsSuccessStatusCode)
            {
                var data = AccountResponseModel.fromJson(await response.Content.ReadAsStringAsync());
                return new JsonResult(data);
            }
            return new JsonResult(await response.Content.ReadAsStringAsync())
            {
                StatusCode = (int)response.StatusCode,
            };
        }

        public async Task<IActionResult> PasswordChange(PasswordUpdateRequestModel updateModel)
        {
            var response = await _client.PostAsJsonAsync("api/Account/PasswordChange", updateModel);
            if (response.IsSuccessStatusCode)
            {
                return new OkResult();
            }
            return new JsonResult(await response.Content.ReadAsStringAsync())
            {
                StatusCode = (int)response.StatusCode,
            };
        }
    }
}