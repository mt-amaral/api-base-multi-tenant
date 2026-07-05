using Api.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Core.Mapping;

public class ClientUserMapping : IEntityTypeConfiguration<ClientUser>
{
    public void Configure(EntityTypeBuilder<ClientUser> builder)
    {
        builder.ToTable("ClientUser");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.UserId).IsUnique();
        builder.HasIndex(x => x.CompanyId);

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<ClientUser>(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .IsRequired();
    }
}
