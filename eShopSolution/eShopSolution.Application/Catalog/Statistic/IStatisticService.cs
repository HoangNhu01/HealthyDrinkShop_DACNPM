using eShopSolution.ViewModels.Statistic;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Statistic
{
    public interface IStatisticService
    {
        public Task<List<RevenueByMonthResult>> GetRevenueByMonth(/*string fromDate, string toDate*/);

        public Task<List<PercentagesOfCategoryResult>> GetPercentagesOfCategory(/*string fromDate, string toDate*/);
        public Task<object> GetOrderStatistic(/*string fromDate, string toDate*/);
        public Task<object> GetUserOrder(/*string fromDate, string toDate*/);
        public Task<object> GetProductOrder(/*string fromDate, string toDate*/);
    }
}
