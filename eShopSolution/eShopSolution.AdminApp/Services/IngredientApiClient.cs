using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Catalog.Ingredients;
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
    public class IngredientApiClient : BaseApiClient, IIngredientApiClient
    {
        public IngredientApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
        }

        public async Task<ApiResult<List<IngredientVm>>> GetAll()
        {
            return await GetAsync<ApiResult<List<IngredientVm>>>("/api/ingredients");
        }

        public async Task<ApiResult<IngredientVm>> GetById(int id)
        {
            return await GetAsync<ApiResult<IngredientVm>>($"/api/ingredients/{id}");
        }
    }
}