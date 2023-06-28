using Azure.Core;
using eShopSolution.Data.Enums;
using eShopSolution.Utilities.Constants;
using eShopSolution.ViewModels.Catalog.Ingredients;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.Sales;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace eShopSolution.AdminApp.Services
{
    public class OrderApiClient : BaseApiClient,IOrderApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OrderApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApiResult<bool>> DeleteOrder(Guid id)
        {
            return await Delete($"/api/orders?orderId={id}");
        }

        public async Task<ApiResult<List<OrderVm>>> GetAll(Guid uId)
        {
            return await GetAsync<ApiResult<List<OrderVm>>>($"/api/orders/customer-order?userId={uId}");
        }

        public async Task<ApiResult<OrderVm>> GetById(Guid id)
        {

            return await GetAsync<ApiResult<OrderVm>>($"/api/orders/{id}");

        }

        public async Task<ApiResult<bool>> UpdateOrderStatus(Guid id, OrderStatus orderStatus)
        {
            var httpContent = new StringContent(String.Empty, Encoding.UTF8, "application/json");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.PutAsync($"/api/orders/{id}/{orderStatus}", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return new ApiSuccessResult<bool>(response.IsSuccessStatusCode);
            }

            return new ApiErrorResult<bool>();
        }
    }
}
