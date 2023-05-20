using eShopSolution.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace eShopSolution.ViewModels.Catalog.Ingredients
{
    public class IngredientVm
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        
        public int Stock { get; set; }
        [JsonIgnore] 
        public virtual List<IngredientInProduct> IngredientInProducts { get; set; }
    }
}
