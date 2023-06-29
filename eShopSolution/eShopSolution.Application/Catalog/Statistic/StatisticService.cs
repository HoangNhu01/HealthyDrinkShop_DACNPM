using eShopSolution.Application.Common;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.Data.Enums;
using eShopSolution.ViewModels.Statistic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

            var results = await _context.OrderDetails
                                .Join(_context.Products,
                                    od => od.ProductId,
                                    p => p.Id,
                                    (od, p) => new { OrderDetails = od, Product = p })
                                .Join(_context.Orders,
                                    combined => combined.OrderDetails.OrderId,
                                    o => o.Id,
                                    (combined, o) => new { Combined = combined, Orders = o })
                                .Where(c => c.Orders.Status == OrderStatus.Success)
                                .GroupBy(x => x.Orders.OrderDate, (x, y) => new RevenueByMonthResult
                                {
                                    OrderCount = y.Count(),
                                    Month = x,
                                    Value = y.Sum(c => c.Combined.OrderDetails.Quantity * (c.Combined.Product.Price - c.Combined.Product.OriginalPrice)),
                                })
                                .ToListAsync();
            return results;
        }

        public async Task<List<PercentagesOfCategoryResult>> GetPercentagesOfCategory(/*string fromDate, string toDate*/)
        {
            var results = await _context.OrderDetails
                        .Join(
                            _context.Products,
                            od => od.ProductId,
                            p => p.Id,
                            (od, p) => new { OrderDetail = od, Product = p }
                        )
                        .Join(
                            _context.ProductInCategories,
                            op => op.Product.Id,
                            o => o.ProductId,
                            (op, o) => new { op.OrderDetail, op.Product, ProductInCategory = o }
                        )
                        .Join(_context.Orders, ot => ot.OrderDetail.OrderId, os => os.Id, (ot, os) => new { ot.OrderDetail, ot.Product, ot.ProductInCategory, Order = os })
                        .Join(_context.Categories, ot => ot.ProductInCategory.CategoryId, os => os.Id, (ot, os) => new { ot.OrderDetail, ot.Product, ot.ProductInCategory, ot.Order, Categories = os })
                        .Join(_context.CategoryTranslations, ot => ot.Categories.Id, os => os.CategoryId, (ot, os) => new { ot.OrderDetail, ot.Product, ot.ProductInCategory, ot.Order,ot.Categories, CategoryTran = os })
                        .Where(c => c.Order.Status == OrderStatus.Success && c.CategoryTran.LanguageId == "vi")
                        .GroupBy(
                            opo => opo.CategoryTran.Name,
                            opo => opo.OrderDetail.Quantity,
                            (name, quantities) => new PercentagesOfCategoryResult() { Name = name, Value = quantities.Sum() }
                        )
                        .ToListAsync();

            
            return results;
        }
        public async Task<object> GetOrderStatistic(/*string fromDate, string toDate*/)
        {
            var orders = await _context.Orders.ToListAsync();
            var today = DateTime.Today;
            var todayOrder = orders.Where(x => x.OrderDate == today).Count();
            var pre_dayOrder = orders.Where(x => x.OrderDate == today.AddDays(-1)).Count() ;


            var revenue_preOrder = orders.Where(x => x.OrderDate.Month == today.Month - 1).Sum(x => x.TotalPrice);
            var revenueTodayOrder = orders.Where(x => x.OrderDate.Month == today.Month).Sum(x => x.TotalPrice);

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
                Cout = revenueTodayOrder,
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
