using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Services
{
    public class CategoryApiClient : BaseApiClient, ICategoryApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CategoryApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResult<CategoryVm>> CreateCategory(CategoryCreateRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.PostAsync("/api/categories/created-categories", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<CategoryVm>>(await response.Content.ReadAsStringAsync());
            }

            return JsonConvert.DeserializeObject<ApiErrorResult<CategoryVm>>(await response.Content.ReadAsStringAsync());
        }

        public async Task<int> DeleteCategory(int id)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.DeleteAsync($"/api/categories/{id}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<int>(body);

            return JsonConvert.DeserializeObject<int>(body);
        }

        public async Task<ApiResult<List<CategoryVm>>> GetAll(string languageId)
        {
            return await GetAsync<ApiResult<List<CategoryVm>>>("/api/categories?languageId=" + languageId);
        }

        public async Task<ApiResult<CategoryVm>> GetById(string languageId, int id)
        {
            return await GetAsync<ApiResult<CategoryVm>>($"/api/categories/{languageId}/{id}");
        }

        public async Task<int> UpdateCategory(CategoryUpdateRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.PutAsync("/api/categories/updated-categories", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
            }

            return JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
        }
    }
}