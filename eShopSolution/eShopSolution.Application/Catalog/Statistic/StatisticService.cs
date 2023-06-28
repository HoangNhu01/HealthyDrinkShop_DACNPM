using eShopSolution.Application.Common;
using eShopSolution.Data.EF;
using eShopSolution.Data.Enums;
using eShopSolution.ViewModels.Statistic;
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

        public StatisticService(EShopDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }
        public async Task<List<RevenueByMonthResult>> GetRevenueByMonth(/*string fromDate, string toDate*/)
        { 
            var fd = DateTime.Parse("02/08/2023");
            var td = DateTime.Parse("08/06/2023");
            var results = await _context.OrderDetails
                                .Join(_context.Products,
                                    od => od.ProductId,
                                    p => p.Id,
                                    (od, p) => new { OrderDetails = od, Product = p })
                                .Join(_context.Orders,
                                    combined => combined.OrderDetails.OrderId,
                                    o => o.Id,
                                    (combined, o) => new { Combined = combined, Orders = o })
                                .Where(c => c.Orders.Status == OrderStatus.Success
                                    && DateTime.Compare(c.Orders.OrderDate,fd) < 0
                                    && DateTime.Compare(c.Orders.OrderDate, td) > 0)
                                .GroupBy(x => x.Orders.OrderDate.Month, (x, y) => new RevenueByMonthResult
                                {
                                    Month = x,
                                    Value = y.Sum(c => c.Combined.OrderDetails.Quantity * (c.Combined.Product.Price - c.Combined.Product.OriginalPrice)),
                                })
                                .ToListAsync();
            return results;
        }
    }
}
