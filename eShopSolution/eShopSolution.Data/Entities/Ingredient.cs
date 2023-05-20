using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace eShopSolution.Data.Entities
{
    public class Ingredient
    {
        public int Id { set; get; }
        public decimal Price { set; get; }
        public int Stock { set; get; }

        public string Name { set; get; }
        public string Description { set; get; }

        public List<IngredientInProduct> IngredientInProducts { get; set; }
    }
}
