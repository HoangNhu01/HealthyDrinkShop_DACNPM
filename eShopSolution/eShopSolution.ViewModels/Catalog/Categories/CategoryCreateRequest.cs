using eShopSolution.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Categories
{
    public class CategoryCreateRequest
    {
        public string Name { set; get; }

        public string Description { set; get; }

        public string Details { set; get; }

        public string SeoDescription { set; get; }

        public string SeoTitle { set; get; }

        public int? ParentId { get; set; }

        public int SortOrder { set; get; }
        public Status Status { get; set; }

        public string SeoAlias { get; set; }

        public string LanguageId { set; get; }
    }
}
