using AREML.EPOD.API.Auth;
using Microsoft.AspNetCore.Authorization;

namespace AREML.EPOD.API.Extensions
{
    public static class PolicyExtension
    {
        public static IServiceCollection AddPolicies(this IServiceCollection services)
        {
            //Role Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                {
                    policy.RequireRole("Admin");
                });

                options.AddPolicy("CustomerPolicy", policy =>
                {
                    policy.RequireRole("Customer");
                });

                options.AddPolicy("ARuserPolicy", policy =>
                {
                    policy.RequireRole("Amararaja User");
                });

                options.AddPolicy("AdminCoordinatorPolicy", policy =>
                    policy.Requirements.Add(new RoleMatchRequirement(
                        includeRoles: new[] { "Admin", "Coordinator" },
                        excludeRoles: new string[] { "Customer","Amararaja User" })));

                options.AddPolicy("ARuserCoordinatorPolicy", policy =>
                    policy.Requirements.Add(new RoleMatchRequirement(
                        includeRoles: new[] { "Amararaja User", "Coordinator" },
                        excludeRoles: new[] { "Admin","Customer" })));

                options.AddPolicy("ARuserCustomerPolicy", policy =>
                    policy.Requirements.Add(new RoleMatchRequirement(
                        includeRoles: new[] { "Amararaja User", "Customer" },
                        excludeRoles: new[] { "Admin", "Coordinator" })));

            });

            services.AddSingleton<IAuthorizationHandler, RoleAuthorizationHandler>();
            return services;
        }
    }
}
