using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Entities
{
    public class OrderDetail
    {

        public Guid OrderId { set; get; }
        public int ProductId { set; get; }
        public int Quantity { set; get; }
        public decimal Price { set; get; }

        public string? ProductName { set; get; }
        public Order Order { get; set; }

        public Product Product { get; set; }

    }
}
