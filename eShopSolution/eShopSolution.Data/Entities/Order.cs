using eShopSolution.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Entities
{
   public class Order
    {
        public Guid Id { set; get; }
        public DateTime OrderDate { set; get; }
        public Guid? UserId { set; get; }
        public string ShipName { set; get; }
        public string ShipAddress { set; get; }
        public string ShipEmail { set; get; }
        public string ShipPhoneNumber { set; get; }
        public decimal TotalPrice { set; get; }
        public OrderStatus Status { set; get; }
        public PaymentStatus PaymentStatus { set; get; }
        public List<OrderDetail> OrderDetails { get; set; }

        public AppUser AppUser { get; set; }



    }
}
