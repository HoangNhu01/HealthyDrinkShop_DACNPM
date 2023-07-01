using eShopSolution.Utilities.Constants;
using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Services
{
    public class ProductApiClient : BaseApiClient, IProductApiClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public ProductApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor; 
            
        }
        public async Task<ApiResult<ProductVm>> CreateProduct(ProductCreateRequest request)
        {
            var sessions = _httpContextAccessor
                .HttpContext
                .Session
                .GetString(SystemConstants.AppSettings.Token);

            var languageId = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();

            if (request.ThumbnailImage != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ThumbnailImage.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ThumbnailImage.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "thumbnailImage", request.ThumbnailImage.FileName);
            }
           
            requestContent.Add(new StringContent(request.Price.ToString()), "price");
            requestContent.Add(new StringContent(request.OriginalPrice.ToString()), "originalPrice");
            requestContent.Add(new StringContent(request.Stock.ToString()), "stock");
            requestContent.Add(new StringContent(request.Name.ToString()), "name");
            requestContent.Add(new StringContent(request.Description.ToString()), "description");

            requestContent.Add(new StringContent((request.Details ?? "").ToString()), "details");
            requestContent.Add(new StringContent(request.SeoDescription.ToString()), "seoDescription");
            requestContent.Add(new StringContent(request.SeoTitle.ToString()), "seoTitle");
            requestContent.Add(new StringContent(request.SeoAlias.ToString()), "seoAlias");
            requestContent.Add(new StringContent(languageId), "languageId");

            var response = await client.PostAsync($"/api/products/", requestContent);
            if(response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var product = JsonConvert.DeserializeObject<ApiSuccessResult<ProductVm>>(result);
                return product;
            }
            else
            {
                return new ApiErrorResult<ProductVm>();
            }

        }
        public async Task<ApiResult<PagedResult<ProductVm>>> GetPagings(GetManageProductPagingRequest request)
        {
            var category = request.CategoryIds.Count > 0 ? $"&CategoryIds={request.CategoryIds.First().ToString()}" : "";
            var data = await GetAsync<ApiResult<PagedResult<ProductVm>>>(
                $"/api/products/paging?pageIndex={request.PageIndex}" +
                $"&pageSize={request.PageSize}" +
                category +
                $"&keyword={request.Keyword}&languageId={request.LanguageId}");

            return data;
        }
        public async Task<bool> UpdateProduct(ProductUpdateRequest request)
        {
            var sessions = _httpContextAccessor
                .HttpContext
                .Session
                .GetString(SystemConstants.AppSettings.Token);

            var languageId = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();

            if (request.ThumbnailImage != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ThumbnailImage.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ThumbnailImage.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "thumbnailImage", request.ThumbnailImage.FileName);
            }

            //requestContent.Add(new StringContent(request.Id.ToString()), "id");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Id.ToString()) ? "" : request.Id.ToString()), "id");

            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Name) ? "" : request.Name.ToString()), "name");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Description) ? "" : request.Description.ToString()), "description");
            requestContent.Add(new StringContent(request.Price.ToString()), "price");
            requestContent.Add(new StringContent(request.OPrice.ToString()), "oPrice");
            requestContent.Add(new StringContent(request.Stock.ToString()), "stock");

            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Details) ? "" : request.Details.ToString()), "details");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.SeoDescription) ? "" : request.SeoDescription.ToString()), "seoDescription");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.SeoTitle) ? "" : request.SeoTitle.ToString()), "seoTitle");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.SeoAlias) ? "" : request.SeoAlias.ToString()), "seoAlias");
            requestContent.Add(new StringContent(languageId), "languageId");

            var response = await client.PutAsync($"/api/products", requestContent);
            return response.IsSuccessStatusCode;
        }


        public async Task<ApiResult<bool>> CategoryAssign(int id, CategoryAssignRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/products/{id}/categories", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public async Task<ApiResult<bool>> IngredientAssign(int id, IngredientAssignRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/products/{id}/ingredients", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public async Task<ApiResult<ProductVm>> GetById(int id, string languageId)
        {
            var data = await GetAsync<ApiResult<ProductVm>>($"/api/products/{id}/{languageId}");

            return data;
        }

        public async Task<ApiResult<bool>> DeleteProduct(int id)
        {
            var result = await Delete($"/api/products/" + id);
            return result;
        }

        public async Task<bool> AddImage(int id, ProductImageCreateRequest request)
        {
            var sessions = _httpContextAccessor
                .HttpContext
                .Session
                .GetString(SystemConstants.AppSettings.Token);

            var languageId = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();

            if (request.ImageFile != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ImageFile.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ImageFile.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "imageFile", request.ImageFile.FileName);
            }
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.IsDefault.ToString()) ? "" : request.IsDefault.ToString()), "isDefault");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.SortOrder.ToString()) ? "" : request.SortOrder.ToString()), "sortOrder");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Caption) ? "" : request.Caption.ToString()), "caption");

            var response = await client.PostAsync($"/api/products/{id}/images", requestContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<ApiResult<bool>> DeleteImage(int imageId)
        {
            var result = await Delete($"/api/products/images/" + imageId);
            return result;
        }
    }
}