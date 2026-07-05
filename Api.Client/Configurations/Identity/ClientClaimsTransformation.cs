using Api.Core.Context;
using Api.Core.Entities.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api.Client.Configurations.Identity;

public class ClientClaimsTransformation(IServiceProvider serviceProvider) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
            return principal;

        var userIdValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdValue))
            return principal;

        if (!long.TryParse(userIdValue, out _))
            return principal;

        using var scope = serviceProvider.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var user = await userManager.FindByIdAsync(userIdValue);
        if (user is null)
            return principal;

        ReplaceClaim(identity, UserClaimNames.UserType, user.UserType.ToString());

        var companyId = await context.ClientUser
            .AsNoTracking()
            .Where(x => x.UserId == user.Id)
            .Select(x => (Guid?)x.CompanyId)
            .FirstOrDefaultAsync();

        if (!companyId.HasValue)
            return principal;

        var companyIdValue = companyId.Value.ToString();
        ReplaceClaim(identity, UserClaimNames.CompanyId, companyIdValue);
        ReplaceClaim(identity, UserClaimNames.TenantId, companyIdValue);

        return principal;
    }

    private static void ReplaceClaim(ClaimsIdentity identity, string claimType, string claimValue)
    {
        var existingClaims = identity.FindAll(claimType).ToList();
        foreach (var claim in existingClaims)
        {
            identity.RemoveClaim(claim);
        }

        identity.AddClaim(new Claim(claimType, claimValue));
    }
}
