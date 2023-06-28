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


    }
}
