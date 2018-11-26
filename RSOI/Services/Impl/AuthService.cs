using System;
using System.Net.Http;
using System.Net.Http.Headers;
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
                return new CreatedResult("",data.ToJson());
            }
            return new JsonResult(await response.Content.ReadAsStringAsync())
            {
                StatusCode = (int)response.StatusCode,
            };
        }

        public async Task<ActionResult<AccountResponseModel>> RefreshToken(string token)
        {
            var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, "api/Account/RefreshToken");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(requestMessage);
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

        public async Task<IActionResult> PasswordChange(PasswordUpdateRequestModel updateModel, string token)
        {
            var requestMessage =
                new HttpRequestMessage(HttpMethod.Post, "api/Account/PasswordChange")
                {
                    Content = new StringContent(updateModel.ToJson())
                };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(requestMessage);
            
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