using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolution.Security
{
    public class AdminRoleHandler : AuthorizationHandler<AdminRoleRequirement>
    {
        public AdminRoleHandler()
        {
            
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRoleRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Fail(); // User is not authenticated
                return;
            }
            var isAdmin = context.User.Claims.FirstOrDefault(x => x.Value.Contains("admin")) != null;
            // Get the user's ID or username, depending on your setup
            //var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (isAdmin)
            {
                context.Succeed(requirement); // User has the "admin" role
            }
            else
            {
                context.Fail(); // User does not have the "admin" role
            }
        }
    }

}
