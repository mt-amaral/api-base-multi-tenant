using Api.Core.Entities.Identity;

namespace Api.Client.Configurations.Setup;

public static class AuthorizationSetup
{
    public static IServiceCollection AddAuthorizationSetup(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Client", policy =>
                policy
                    .RequireAuthenticatedUser()
                    .RequireClaim(UserClaimNames.UserType, UserType.Client.ToString()));
        });

        return services;
    }
}
