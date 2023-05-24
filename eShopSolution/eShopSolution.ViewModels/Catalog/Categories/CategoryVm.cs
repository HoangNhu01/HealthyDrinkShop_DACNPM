using eShopSolution.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace eShopSolution.ViewModels.Catalog.Categories
{
    public class CategoryVm
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }
        [JsonIgnore]
        public virtual List<ProductInCategory> ProductInCategories { get; set; }
    }
}
