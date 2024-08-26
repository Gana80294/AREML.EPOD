using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AREML.EPOD.API.Auth
{
    public class RoleMatchRequirement : IAuthorizationRequirement
    {
        public List<string> IncludeRoles { get; }
        public List<string> ExcludeRoles { get; }

        public RoleMatchRequirement(IEnumerable<string> includeRoles, IEnumerable<string> excludeRoles)
        {
            IncludeRoles = includeRoles.ToList();
            ExcludeRoles = excludeRoles.ToList();
        }
    }

    public class RoleAuthorizationHandler : AuthorizationHandler<RoleMatchRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleMatchRequirement requirement)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            var userRoles = context.User.FindAll(ClaimTypes.Role).Select(role => role.Value);

            bool hasIncludedRole = requirement.IncludeRoles.Any(includeRole =>
                userRoles.Any(userRole => userRole.Contains(includeRole, StringComparison.OrdinalIgnoreCase)));

            bool hasExcludedRole = requirement.ExcludeRoles.Any(excludeRole =>
                userRoles.Any(userRole => userRole.Contains(excludeRole, StringComparison.OrdinalIgnoreCase)));

            if (hasIncludedRole && !hasExcludedRole)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
