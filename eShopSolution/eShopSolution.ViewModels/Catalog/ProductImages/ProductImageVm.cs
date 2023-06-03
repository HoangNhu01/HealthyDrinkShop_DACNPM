using eShopSolution.Data.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace eShopSolution.ViewModels.Catalog.ProductImages
{
    public class ProductImageVm
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ImagePath { get; set; }

        public string Caption { get; set; }

        public bool IsDefault { get; set; }

        public DateTime DateCreated { get; set; }

        public int SortOrder { get; set; }

        public long FileSize { get; set; }

        public byte[] Data { get; set; }

        [JsonIgnore]        
        public Product Product { get; set; }
    }
}