using Api.Core.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Core.Entities;

[Index(nameof(UserId), IsUnique = true)]
public class UserSession
{
    public long Id { get; init; }
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime LastAccessAtUtc { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }

    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }

    public bool IsActive => RevokedAtUtc == null && ExpiresAtUtc > DateTime.UtcNow;

    public UserSession() { }

    public UserSession(long userId, string? ipAddress, string? userAgent)
    {
        UserId = userId;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        CreatedAtUtc = DateTime.UtcNow;
        LastAccessAtUtc = DateTime.UtcNow;
        ExpiresAtUtc = DateTime.UtcNow.AddDays(7);
    }

    public void Refresh(string? ipAddress, string? userAgent)
    {
        LastAccessAtUtc = DateTime.UtcNow;
        ExpiresAtUtc = DateTime.UtcNow.AddDays(7);
        RevokedAtUtc = null;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }

    public void Touch()
    {
        LastAccessAtUtc = DateTime.UtcNow;
    }

    public void Revoke()
    {
        if (RevokedAtUtc is not null) return;
        RevokedAtUtc = DateTime.UtcNow;
    }
}
