using Api.Core.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Core.Mapping.Identity;

public class RoleClaimMapping : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        builder.ToTable("IdentityRoleClaim");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ClaimType).HasMaxLength(100);
        builder.Property(x => x.ClaimValue).HasMaxLength(200);
    }
}