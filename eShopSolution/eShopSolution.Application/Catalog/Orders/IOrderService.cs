using eShopSolution.Data.Enums;
using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.Sales;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Sales.Orders
{
    public interface IOrderService
    {
        Task<ApiResult<List<OrderVm>>> GetByUserId(Guid userId);

        Task<ApiResult<OrderVm>> GetById(Guid orderId);

        Task<ApiResult<Guid>> Create(CheckOutRequest request);

        Task<ApiResult<int>> UpdateStatus(Guid orderId, OrderStatus orderStatus);

        Task<ApiResult<bool>> Delete(Guid orderId);


    }
}
