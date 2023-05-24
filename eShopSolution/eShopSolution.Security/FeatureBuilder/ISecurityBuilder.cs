using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eShopSolution.Security.FeatureBuilder
{
    public interface ISecurityBuilder
    {
        IServiceCollection Services { get; set; }
        IConfiguration Configuration { get; set; }
    }
}
