using eShopSolution.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Statistic
{
    public class UserOrderResult
    {
        public object Name { get; set; }
        public object Id { get; set; }
        public object Product { get; set; }
        public object Payments { get; set; }
        public OrderStatus Status { get; set; }
    }

}
