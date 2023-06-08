using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Catalog.Ingredients;
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
    public class IngredientApiClient : BaseApiClient, IIngredientApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IngredientApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResult<bool>> CreateIngredient(IngredientCreateRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.PostAsync("/api/ingredients", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return new ApiSuccessResult<bool>(response.IsSuccessStatusCode);
            }

            return new ApiErrorResult<bool>();
        }

        public async Task<ApiResult<bool>> DeleteIngredient(int ngredientId)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.DeleteAsync($"/api/ingredients?ingredientId={ngredientId}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return new ApiSuccessResult<bool>(response.IsSuccessStatusCode);
            }

            return new ApiErrorResult<bool>();
        }

        public async Task<ApiResult<List<IngredientVm>>> GetAll()
        {
            return await GetAsync<ApiResult<List<IngredientVm>>>("/api/ingredients");
        }

        public async Task<ApiResult<IngredientVm>> GetById(int id)
        {
            return await GetAsync<ApiResult<IngredientVm>>($"/api/ingredients/{id}");
        }

        public async Task<ApiResult<bool>> UpdateIngredient(IngredienUpdateRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.PutAsync("/api/ingredients", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return new ApiSuccessResult<bool>(response.IsSuccessStatusCode);
            }

            return new ApiErrorResult<bool>();
        }
    }
}