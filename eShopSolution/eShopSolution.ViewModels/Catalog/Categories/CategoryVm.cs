using eShopSolution.Data.Entities;
using eShopSolution.Data.Enums;
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

        public string SeoDescription { set; get; }

        public string SeoTitle { set; get; }

        public string SeoAlias { get; set; }

        public int SortOrder { set; get; }
        public string LanguageId { set; get; }

        public Status Status { set; get; }  
        [JsonIgnore]
        public virtual List<ProductInCategory> ProductInCategories { get; set; }
    }
}
