using Azure.Core;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.ExternalUser;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Services
{
    public class UserApiClient : IUserApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static HttpClient client = new HttpClient();
        private const string oauthFbBaseUri = "https://graph.facebook.com/";
        private const string oauthGgBaseUri = "https://accounts.google.com/o/oauth2/token";
        
        public UserApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApiResult<string>> Authenticate(LoginRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.PostAsync("/api/users/authenticate", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<string>>(await response.Content.ReadAsStringAsync());
            }

            return JsonConvert.DeserializeObject<ApiErrorResult<string>>(await response.Content.ReadAsStringAsync());
        }

        public async Task<ApiResult<bool>> Delete(Guid id)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.DeleteAsync($"/api/users/{id}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }

        public async Task<ValidateTokenResult> GetAccessFBTokenAsync(string facebookAppId, string facebookAppSecret, string facebookAppDomain, string code)
        {
            HttpResponseMessage response = await client.GetAsync(oauthFbBaseUri + "oauth/access_token?client_id=" + facebookAppId + "&client_secret=" + facebookAppSecret + "&redirect_uri=" + facebookAppDomain + "&code=" + code);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                string result = JToken.Parse(json).ToString();

                var accessToken = JsonConvert.DeserializeObject<ValidateTokenResult>(result);

                return accessToken;
            }

            return new ValidateTokenResult();
        }

        public async Task<ApiResult<UserVm>> GetById(Guid id)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/users/{id}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<UserVm>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<UserVm>>(body);
        }

        public async Task<GoogleUserInfor> GetGgUserAsync(string accessToken)
        {
            HttpResponseMessage response = await client.GetAsync("https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token=" + accessToken);

            if (response.IsSuccessStatusCode)
            {
                string strJson = response.Content.ReadAsStringAsync().Result;
                var user = JsonConvert.DeserializeObject<GoogleUserInfor>(strJson);
                user.Access_token = accessToken;

                return user;
            }

            return new GoogleUserInfor();
        }
        public async Task<FaceBookUserInfor> GetFbUserAsync(string accessToken)
        {
            HttpResponseMessage response = await client.GetAsync(oauthFbBaseUri + "me?fields=id,name,first_name,last_name,email,birthday,picture&access_token=" + accessToken);

            if (response.IsSuccessStatusCode)
            {
                string strJson = response.Content.ReadAsStringAsync().Result;
                var user = JsonConvert.DeserializeObject<FaceBookUserInfor>(strJson);
                user.Access_token = accessToken;

                return user;
            }

            return new FaceBookUserInfor();
        }
        public async Task<ApiResult<PagedResult<UserVm>>> GetUsersPagings(GetUserPagingRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/users/paging?pageIndex=" +
                $"{request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}");
            var body = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<ApiSuccessResult<PagedResult<UserVm>>>(body);
            return users;
        }

        public async Task<ApiResult<bool>> RegisterUser(RegisterRequest registerRequest)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);

            var json = JsonConvert.SerializeObject(registerRequest);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/users", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public async Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/users/{id}/roles", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public async Task<ApiResult<bool>> UpdateUser(Guid id, UserUpdateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/users/{id}", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public async Task<ApiResult<UserVm>> GetByName(string userName)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/users/searching/{userName}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<UserVm>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<UserVm>>(body);
        }

        public async Task<ApiResult<string>> ExternalAuthenticate(FaceBookUserInfor request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.PostAsync("/api/users/external-fb-authenticate", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<string>>(await response.Content.ReadAsStringAsync());
            }

            return JsonConvert.DeserializeObject<ApiErrorResult<string>>(await response.Content.ReadAsStringAsync());
        }
        public async Task<ApiResult<string>> ExternalAuthenticate(GoogleUserInfor request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.PostAsync("/api/users/external-gg-authenticate", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<string>>(await response.Content.ReadAsStringAsync());
            }

            return JsonConvert.DeserializeObject<ApiErrorResult<string>>(await response.Content.ReadAsStringAsync());
        }
        public async Task<ValidateTokenResult> GetAccessGGTokenAsync(string googleClientId, string googleAppSecret, string googleAppDomai, string code)
        {
            var postData = new
            {
                code = code,
                client_id = googleClientId,
                client_secret = googleAppSecret,
                redirect_uri = googleAppDomai,
                grant_type = "authorization_code"
            };
            var json = JsonConvert.SerializeObject(postData);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(oauthGgBaseUri, httpContent);
            if (response.IsSuccessStatusCode)
            {
                string jsonrp = await response.Content.ReadAsStringAsync();
                string result = JToken.Parse(jsonrp).ToString();

                var accessToken = JsonConvert.DeserializeObject<ValidateTokenResult>(result);

                return accessToken;
            }

            return new ValidateTokenResult();

        }

    }
}