using eShopSolution.Application.Common;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.Data.Enums;
using eShopSolution.ViewModels.Statistic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eShopSolution.ViewModels.AppSystem.ExternalUser.FaceBookUserInfor;

namespace eShopSolution.Application.Catalog.Statistic
{
    public class StatisticService : IStatisticService
    {
        private readonly EShopDbContext _context;
        private readonly IStorageService _storageService;
        private readonly UserManager<AppUser> _userManager;
        public StatisticService(EShopDbContext context, IStorageService storageService, UserManager<AppUser> userManager)
        {
            _context = context;
            _storageService = storageService;
            _userManager = userManager;
        }
        public async Task<List<RevenueByMonthResult>> GetRevenueByMonth(/*string fromDate, string toDate*/)
        {
                                    var results =await _context.OrderDetails
                                .Join(_context.Products,
                                      od => od.ProductId,
                                      p => p.Id,
                                      (od, p) => new { OrderDetail = od, Product = p })
                                .Join(_context.Orders,
                                      odp => odp.OrderDetail.OrderId,
                                      o => o.Id,
                                      (odp, o) => new { OrderDetailAndProduct = odp, Order = o })
                                .GroupBy(odp => odp.Order.OrderDate.Date, (odp, x) => new RevenueByMonthResult
                                {
                                    Month = odp,
                                    OrderCount = x.Count(),
                                    Value = x.Sum(odp => odp.OrderDetailAndProduct.OrderDetail.Quantity *
                                                        (odp.OrderDetailAndProduct.Product.Price -
                                                         odp.OrderDetailAndProduct.Product.OriginalPrice)),


                                })
                                .ToListAsync();
            return results;
        }

        public async Task<List<PercentagesOfCategoryResult>> GetPercentagesOfCategory(/*string fromDate, string toDate*/)
        {
            var results = await _context.Categories
                        .Join(_context.CategoryTranslations, c => c.Id, ct => ct.CategoryId, (c, ct) => new { Category = c, CategoryTranslation = ct })
                        .Join(_context.ProductInCategories, x => x.Category.Id, pc => pc.CategoryId, (x, pc) => new { x.Category, x.CategoryTranslation, ProductInCategory = pc })
                        .Join(_context.Products, y => y.ProductInCategory.ProductId, p => p.Id, (y, p) => new { y.Category, y.CategoryTranslation, y.ProductInCategory, Product = p })
                        .Join(_context.OrderDetails, a => a.Product.Id, od => od.ProductId, (a, od) => new { a.Category, a.CategoryTranslation, a.ProductInCategory, a.Product, OrderDetail = od })
                        .Join(_context.Orders, o => o.OrderDetail.OrderId, d => d.Id, (d, o) => new { d.Category, d.CategoryTranslation, d.ProductInCategory,d.Product, d.OrderDetail, Order = o })
                        .GroupBy(g => new { g.Category.Id, g.CategoryTranslation.Name }, (name,x) => new PercentagesOfCategoryResult()
                        {
                            Name = name,
                            Value = x.Sum(x => x.OrderDetail.Quantity)

                        })
                        
                        .ToListAsync();

            
            return results;
        } 
        public async Task<object> GetUserOrder(/*string fromDate, string toDate*/)
        {
            var results = await _context.Orders.Include(x => x.AppUser).Include(x => x.OrderDetails).Where(x => x.OrderDate.Date == DateTime.Now.Date)
                          .Select(x => new /*UserOrderResult*/
                          {
                              Id = x.Id,
                              Name = x.AppUser.UserName,
                              Product =String.Join(" ", x.OrderDetails.Select(x => x.ProductName)),
                              Payments = String.Format(new CultureInfo("vi-VN"), "{0:C}", x.TotalPrice),
                              Status = x.Status.ToString(),
                          })
                        .ToListAsync();       
            return results;
        } 
        public async Task<object> GetProductOrder(/*string fromDate, string toDate*/)
        {
            var results = await _context.Products
                         
                         .Join(_context.OrderDetails, a => a.Id, od => od.ProductId, (a, od) => new {  Product = a, OrderDetail = od })
                         .Join(_context.Orders, o => o.OrderDetail.OrderId, d => d.Id, (d, o) => new { d.Product, d.OrderDetail, Order = o })
                         .Join(_context.ProductImages, p => p.Product.Id, pi => pi.ProductId, (d, o) => new { d.Product, d.OrderDetail, d.Order, ProductImage = o })
                         .Join(_context.ProductTranslations, p => p.Product.Id, pt => pt.ProductId, (d, o) => new { d.Product, d.OrderDetail, d.Order, d.ProductImage, ProductTranslation = o })
                         .Where(x => x.ProductImage.IsDefault && x.Order.OrderDate.Date == DateTime.Now.Date)
                         .GroupBy(g => new { g.Product.Id, g.ProductTranslation.Name, g.ProductImage.Data, g.Product.Price}, (name, x) => new
                         {
                             Id = name.Id,
                             Name = name.Name,
                             Image = name.Data,
                             Price = String.Format(new CultureInfo("vi-VN"), "{0:C}", name.Price)
,
                             Sold = x.Sum(x => x.OrderDetail.Quantity),
                             Revenue = String.Format(new CultureInfo("vi-VN"), "{0:C}", x.Sum(x =>(x.OrderDetail.Quantity * (x.Product.Price - x.Product.OriginalPrice))))

                         })

                         .ToListAsync();


            return results;
        }
        public async Task<object> GetOrderStatistic(/*string fromDate, string toDate*/)
        {
            var orders = await _context.Orders.ToListAsync();
            var today = DateTime.Today;
            var todayOrder = orders.Where(x => x.OrderDate.Day == today.Day).Count();
            var pre_dayOrder = orders.Where(x => x.OrderDate == today.AddDays(-1)).Count() ;


            var revenue_preOrder = orders.Where(x => x.OrderDate.Month == today.Month - 1).Sum(x => x.TotalPrice);
            var revenueTodayOrder = orders.Where(x => x.OrderDate.Month == today.Month && x.Status == OrderStatus.Success).Sum(x => x.TotalPrice);

            var users = await _userManager.Users.ToListAsync();
            var todayUser = users.Where(x => x.DayCreate.Year == today.Year).Count();
            var preUser = users.Where(x => x.DayCreate.Year == today.Year -1).Count();

            var order = new OrderSatistic()
            {
                Cout = todayOrder,
                OrderPercentage = todayOrder * 100/ (pre_dayOrder == 0 ? 1 : pre_dayOrder) +"%",
            };
            var revenue = new RevenueSatistic()
            {
                Cout = String.Format(new CultureInfo("vi-VN"), "{0:C}", (int)Math.Round(revenueTodayOrder))
,
                RevenuePercentage = Math.Round(revenueTodayOrder * 100 / (revenue_preOrder == 0 ? 100000 : revenue_preOrder)) + "%",
            };
            var user = new UserSatistic()
            {
                Cout = todayUser,
                UserPercentage = todayUser * 100/ (preUser == 0 ? 1 : preUser) + "%",
            };
            return new {Order = order, Revenue = revenue, User = user};
        }
    }
}
