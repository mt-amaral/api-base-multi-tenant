using Api.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Core.Mapping;

public class AdminUserMapping : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> builder)
    {
        builder.ToTable("AdminUser");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.UserId).IsUnique();
        builder.HasIndex(x => x.CompanyId);

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<AdminUser>(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .IsRequired();
    }
}
