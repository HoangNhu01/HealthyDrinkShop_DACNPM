using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.Data.Enums;
using eShopSolution.Utilities.Exceptions;
using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.Sales;
using eShopSolution.ViewModels.System.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Sales.Orders
{
    public class OrderService : IOrderService
    {
        private readonly EShopDbContext _context;

        public OrderService(EShopDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResult<Guid>> Create(CheckOutRequest request)
        {
            var newOrder = new Order()
            {
                Id = request.OrderId,
                UserId = request.UserId,
                OrderDate = request.OrderDate,
                ShipAddress = request.Address,
                ShipEmail = request.Email,
                ShipPhoneNumber = request.PhoneNumber,
                ShipName = request.UserName,
                Status = (Data.Enums.OrderStatus)1,
                PaymentStatus = (PaymentStatus)request.PaymentStatus,
                TotalPrice = request.TotalPrice,
                OrderDetails = new List<OrderDetail>(),
                
            };
            newOrder.OrderDetails = request.CartItems.Select(x => new OrderDetail()
            {
                Price = x.Price,
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                ProductName = x.Name,
            }).ToList();
            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<Guid>()
            {
                ResultObj = newOrder.Id
            };
        }

        public async Task<ApiResult<bool>> Delete(Guid orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new EShopException($"Can not find order by {orderId}");
            var res = _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            if(res != null)
            {
                return new ApiSuccessResult<bool>(true,"Xóa đơn hàng thành công");
            }
            else
            {
                return new ApiErrorResult<bool>("Xóa sản phẩm thất bại");
            }
        }

        public async Task<ApiResult<List<OrderVm>>> GetAll(string userName)
        {
            var order = await _context.Orders.Include(x => x.OrderDetails)
                                             .Where(x => x.ShipName.Contains(userName?? ""))
                                             .Select(x => new OrderVm()
                                               {
                                                   Id = x.Id,
                                                   UserId = x.UserId,
                                                   OrderDate = x.OrderDate,
                                                   ShipAddress = x.ShipAddress,
                                                   ShipEmail =x.ShipEmail,
                                                   ShipPhoneNumber = x.ShipPhoneNumber,
                                                   TotalPrice = x.TotalPrice,
                                                   ShipName = x.ShipName,
                                                   OrderStatus = x.Status,
                                               }).ToListAsync();

            return new ApiSuccessResult<List<OrderVm>>()
            {
                ResultObj = order
            };
            
        }

        public async Task<ApiResult<OrderVm>> GetById(Guid id)
        {
            var order = await _context.Orders.Include(x => x.OrderDetails)
                                             .ThenInclude(x=> x.Product)
                                             .ThenInclude(x =>x.ProductTranslations).FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
                throw new EShopException($"Can not find order by {id}");
            return new ApiSuccessResult<OrderVm>()
            {
                ResultObj = new OrderVm()
                {
                    Id = id,
                    UserId = order.UserId,
                    OrderDate = order.OrderDate,
                    ShipAddress = order.ShipAddress,
                    ShipEmail = order.ShipEmail,
                    ShipPhoneNumber = order.ShipPhoneNumber,
                    TotalPrice = order.TotalPrice,
                    ShipName = order.ShipName,
                    OrderStatus = order.Status,
                    OrderDetails = order.OrderDetails
                }
            };
        }

        public async Task<ApiResult<int>> UpdateStatus(Guid orderId, OrderStatus orderStatus)
        {
            var order = await _context.Orders.Include(x => x.OrderDetails).FirstOrDefaultAsync(x => x.Id == orderId);
            order.Status = orderStatus;
            var result = await _context.SaveChangesAsync();
            return new ApiSuccessResult<int>(result,"Cập nhật trạng thái đơn hàng thàng công");
        }

    }
}
