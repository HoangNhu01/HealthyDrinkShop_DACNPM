using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.System.ExternalUser
{
    public class GoogleUserInfor 
    {
        public string Access_token { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Given_name { get; set; }
        public string Family_name { get; set; }
        public string Locale { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
        public bool Verified_email { get; set; }
             
    }
}
