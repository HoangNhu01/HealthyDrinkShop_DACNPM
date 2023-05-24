using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eShopSolution.Security.FeatureBuilder
{
    public class DefaultSecurityBuilder : ISecurityBuilder
    {
        public DefaultSecurityBuilder(IServiceCollection services, IConfiguration configuration)
        {
            Services = services;
            Configuration = configuration;
            }
        public IServiceCollection Services { get; set; }
        public IConfiguration Configuration { get; set; }
    }
}
