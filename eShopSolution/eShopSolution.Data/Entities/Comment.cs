using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Entities
{
    public class Comment
    {
        public Guid Id { set; get; }

        public Guid UserId { set; get; }

        public string CommentText { set; get; }

        public DateTime? CreatedDate { set; get; }

        public Guid? ParentId { set; get; }
        public int ProductId { set; get; }

        public AppUser User { set; get; }
        public Product Product { set; get; }
    }
}
