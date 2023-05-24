using System;
using System.Collections.Generic;
using System.Text;
using eShopSolution.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eShopSolution.Security.FeatureBuilder
{
    public static class SecurityServiceCollectionExtensions
    {
        public static ISecurityBuilder AddSecurityFeature(this IServiceCollection services, IConfiguration configuration)
        {
            return new DefaultSecurityBuilder(services, configuration);
        }
        public static ISecurityBuilder AddManagers(this ISecurityBuilder builder)
        {
            builder.Services.AddSingleton<IAuthorizationHandler, AdminRoleHandler>();
            return builder;
        }
    }
}
