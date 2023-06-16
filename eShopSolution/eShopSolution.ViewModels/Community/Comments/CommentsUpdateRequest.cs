using eShopSolution.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Community.Comments
{
    public class CommentUpdateRequest
    {
        public Guid Id { set; get; }

        public string CommentText { set; get; }

        public Guid? ParentId { set; get; }


    }
}
