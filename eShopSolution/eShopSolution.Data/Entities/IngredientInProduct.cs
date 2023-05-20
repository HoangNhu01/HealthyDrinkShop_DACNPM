using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Entities
{
    public class IngredientInProduct
    {
        public int IngredientId { get; set; }
        public int ProductId { get; set; }
        public virtual Ingredient Ingredient { get; set; }
        public virtual Product  Product { get; set; }
    }
}
