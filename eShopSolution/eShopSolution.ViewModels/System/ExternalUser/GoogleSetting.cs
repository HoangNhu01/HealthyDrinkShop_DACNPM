using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.System.ExternalUser
{
    public class GoogleSetting
    {
        public string GoogleClientId { get; set; }
        public string GoogleRedirectUri { get; set; }
        public string Scope { get; set; }
    }
}
