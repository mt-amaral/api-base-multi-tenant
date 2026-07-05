using Api.Core.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Core.Mapping.Identity;

public class RoleMapping : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("IdentityRole");

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.NormalizedName).IsUnique();

        builder.Property(x => x.Name).HasMaxLength(180);
        builder.Property(x => x.NormalizedName).HasMaxLength(180);
        builder.Property(x => x.Description).HasMaxLength(300);
        builder.Property(x => x.ConcurrencyStamp).IsConcurrencyToken();
    }
}