﻿using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.System.ExternalUser
{
    public class ValidateTokenResult
	{
        public string Access_token { get; set; }
        public string Token_type { get; set; }
        public string Expires_in { get; set; }
        public string Refresh_token { get; set; }
    }
}
