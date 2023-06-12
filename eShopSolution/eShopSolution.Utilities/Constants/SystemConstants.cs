using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Utilities.Constants
{
    public class SystemConstants
    {
        public const string MainConnectionString = "eShopSolutionDb";

        public const string CacheConnectionString = "Redis";
        public class AppSettings
        {
            public const string DefaultLanguageId = "DefaultLanguageId";
            public const string Token = "Token";
            public const string BaseAddress = "BaseAddress";
        }
        public class PaymentApi
        {
            public const string Url = "VNpayApi:Url";
            public const string ReturnUrl = "VNpayApi:ReturnUrl";
            public const string TmnCode = "VNpayApi:TmnCode";
            public const string HashSecret = "VNpayApi:HashSecret";
        }
    }
}
