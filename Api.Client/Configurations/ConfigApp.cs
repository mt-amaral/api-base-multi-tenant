using System.Security.Claims;

namespace Api.Client.Configurations;

public static class ConfigApp
{
    public static class Cors
    {
        public static readonly string[] DevOrigins =
        [
            "http://localhost:4200"
        ];

        public static readonly string[] ProdOrigins =
        [
            "http://localhost:5003",
            "http://localhost:5003/"
        ];
    }

    public static int PasswordRequiredLength = 8;
    public static int PasswordRequiredUniqueChars = 1;
    public static bool RequireConfirmedAccount = false;
    public static string AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    public static int LockoutMaxFailedAccessAttempts = 5;
    public static TimeSpan LockoutDefaultTimeSpan = TimeSpan.FromMinutes(20);
    public static string UserIdClaimType = ClaimTypes.NameIdentifier;
    public static string UserNameClaimType = ClaimTypes.Name;
    public static string RoleClaimType = ClaimTypes.Role;
    public static string RefreshTokenCookieName = "clientRefreshToken";
    public static int TokenCookieTime = 15;
    public static int RefreshTokenCookieTime = 300;
}
