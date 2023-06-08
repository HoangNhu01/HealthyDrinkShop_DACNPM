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
        Task<ApiResult<List<OrderVm>>> GetAll(string userName, Guid id);

        Task<ApiResult<OrderVm>> GetById(int id);

        Task<ApiResult<int>> Create(CheckOutRequest request);

        Task<ApiResult<int>> UpdateStatus(int orderId, OrderStatus orderStatus);

        Task<ApiResult<bool>> Delete(int orderId);


    }
}
