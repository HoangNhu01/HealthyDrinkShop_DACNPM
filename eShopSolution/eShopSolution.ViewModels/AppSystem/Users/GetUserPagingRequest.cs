using eShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.AppSystem.Users
{
    public class GetUserPagingRequest : PagedResultBase
    {
        public string Keyword { get; set; }
    }
}