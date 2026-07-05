using Api.Core.Context;
using Api.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Api.Client.Configurations.Setup;

public static class IdentitySetup
{
    public static IServiceCollection AddIdentitySetup(this IServiceCollection services)
    {
        services.AddIdentity<User, Role>(options =>
        {
            options.Password.RequiredLength = ConfigApp.PasswordRequiredLength;
            options.Password.RequiredUniqueChars = ConfigApp.PasswordRequiredUniqueChars;
            options.SignIn.RequireConfirmedAccount = ConfigApp.RequireConfirmedAccount;
            options.User.AllowedUserNameCharacters = ConfigApp.AllowedUserNameCharacters;
            options.Lockout.MaxFailedAccessAttempts = ConfigApp.LockoutMaxFailedAccessAttempts;
            options.Lockout.DefaultLockoutTimeSpan = ConfigApp.LockoutDefaultTimeSpan;
            options.ClaimsIdentity.UserIdClaimType = ConfigApp.UserIdClaimType;
            options.ClaimsIdentity.UserNameClaimType = ConfigApp.UserNameClaimType;
            options.ClaimsIdentity.RoleClaimType = ConfigApp.RoleClaimType;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Login";
            options.LogoutPath = "/Logout";
            options.Cookie.Name = ".ApiClient.Identity";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.None;
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            options.Events.OnRedirectToLogin = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                }

                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                }

                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            };
        });

        return services;
    }
}
