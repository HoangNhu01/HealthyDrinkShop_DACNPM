using eShopSolution.Data.Entities;
using eShopSolution.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace eShopSolution.ViewModels.Community.Comments
{
    public class CommentVm
    {
        public Guid Id { set; get; }

        public Guid UserId { set; get; }

        public string UserName { set; get; }
        public string CommentText { set; get; }

        public DateTime? CreatedDate { set; get; }

        public Guid? ParentId { set; get; }
        public int ProductId { set; get; }
        public string ProductName { set; get; }
        public AppUser User { set; get; }
        public Product Product { set; get; }
    }
}
