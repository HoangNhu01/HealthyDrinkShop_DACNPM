using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Services
{
    public class CategoryApiClient : BaseApiClient, ICategoryApiClient
    {
        public CategoryApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
        }

        public async Task<ApiResult<List<CategoryVm>>> GetAll(string languageId)
        {
            return await GetAsync<ApiResult<List<CategoryVm>>>("/api/categories?languageId=" + languageId);
        }

        public async Task<ApiResult<CategoryVm>> GetById(string languageId, int id)
        {
            return await GetAsync<ApiResult<CategoryVm>>($"/api/categories/{id}/{languageId}");
        }
    }
}