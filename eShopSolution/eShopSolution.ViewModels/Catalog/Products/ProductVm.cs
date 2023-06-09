using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using eShopSolution.Data.Entities;


namespace eShopSolution.ViewModels.Catalog.Products
{
    public class ProductVm
    {
        public ProductVm()
        {
            
        }
        public int Id { set; get; }
        public decimal Price { set; get; }
        public decimal OriginalPrice { set; get; }
        public int Stock { set; get; }
        public int ViewCount { set; get; }
        public DateTime DateCreated { set; get; }

        public string Name { set; get; }
        public string Description { set; get; }
        public string Details { set; get; }
        public string SeoDescription { set; get; }
        public string SeoTitle { set; get; }

        public bool IsFeature { set; get; }
        public string SeoAlias { get; set; }
        public string LanguageId { set; get; }
        public byte[]? ThumbnailImage { set; get; } 

        public List<byte[]>? ListImg { set; get; } = new List<byte[]>();
        [JsonIgnore]
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProductInCategory> ProductInCategories { get; set; } = new List<ProductInCategory>();
        [JsonIgnore]
        public virtual ICollection<IngredientInProduct> IngredientInProducts { get; set; } = new List<IngredientInProduct>();
    }
}
