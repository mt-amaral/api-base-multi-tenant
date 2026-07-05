using Api.Core.Entities;
using Api.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Api.Core.Context;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<
    User,
    Role,
    long,
    IdentityUserClaim<long>,
    IdentityUserRole<long>,
    IdentityUserLogin<long>,
    RoleClaim,
    IdentityUserToken<long>>(options)
{
    public DbSet<User> User => Set<User>();
    public DbSet<Company> Company => Set<Company>();
    public DbSet<AdminUser> AdminUser => Set<AdminUser>();
    public DbSet<ClientUser> ClientUser => Set<ClientUser>();
    public DbSet<RefreshToken> RefreshToken => Set<RefreshToken>();
    public DbSet<UserSession> UserSession => Set<UserSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}


