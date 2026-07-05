using Api.Core.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Core.Entities;

[Index(nameof(Token), IsUnique = true)]
[Index(nameof(UserId), IsUnique = true)]
[Table("RefreshToken")]
public class RefreshToken
{
    public RefreshToken() { }

    public RefreshToken(string token, long userId)
    {
        Token = token;
        UserId = userId;
        CreatedAtUtc = DateTime.UtcNow;
        ExpiresAtUtc = DateTime.UtcNow.AddDays(7);
    }

    public long Id { get; init; }

    [MaxLength(512)]
    public string Token { get; private set; } = string.Empty;

    public long UserId { get; private set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; private set; } = null!;

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime ExpiresAtUtc { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }


    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;


    public bool IsRevoked => RevokedAtUtc.HasValue;

    public bool IsActive => !IsExpired && !IsRevoked;

    public void Revoke()
    {
        if (IsRevoked) return;
        RevokedAtUtc = DateTime.UtcNow;
    }

    public void Replace(string newToken)
    {
        Token = newToken;
        CreatedAtUtc = DateTime.UtcNow;
        ExpiresAtUtc = DateTime.UtcNow.AddDays(7);
        RevokedAtUtc = null;
    }
}
