using Api.Configurations.Identity;

namespace Api.Configurations.Setup;

public static class AuthorizationSetup
{
    public static IServiceCollection AddAuthorizationSetup(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            foreach (var permission in Permissions.GetPermissions())
            {
                options.AddPolicy(permission.PermissionName, policy =>
                    policy.RequireClaim(permission.ClaimType, permission.PermissionName));
            }
        });

        return services;
    }
}
