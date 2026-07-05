using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Configurations.Setup;

public static class RateLimiterSetup
{
    public static IServiceCollection AddRateLimiterSetup(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // TODO fazer mais testes
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy("api", httpContext =>
            {
                string GetRealIp()
                {
                    if (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
                    {
                        var ip = forwardedFor.FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();
                        if (!string.IsNullOrEmpty(ip)) return ip;
                    }
                    return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                }

                string key;
                bool isAuthenticated = httpContext.User.Identity?.IsAuthenticated == true;

                if (isAuthenticated)
                {
                    var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    key = userId ?? GetRealIp();
                }
                else
                {
                    var userAgent = httpContext.Request.Headers.UserAgent.ToString();
                    key = $"{GetRealIp()}:{userAgent}";
                }

                var permitLimit = isAuthenticated
                    ? ConfigApp.RateLimitPermitLimitAuthenticated
                    : ConfigApp.RateLimitPermitLimitAnonymous;

                var queueLimit = isAuthenticated
                    ? ConfigApp.RateLimitQueueLimitAuthenticated
                    : ConfigApp.RateLimitQueueLimitAnonymous;

                return RateLimitPartition.GetSlidingWindowLimiter(key, _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = permitLimit,
                    Window = TimeSpan.FromSeconds(ConfigApp.RateLimitWindowSeconds),
                    SegmentsPerWindow = ConfigApp.RateLimitSegmentsPerWindow,
                    QueueProcessingOrder = ConfigApp.QueueProcessingOrder,
                    QueueLimit = queueLimit,
                    AutoReplenishment = true
                });
            });
        });

        return services;
    }
}
