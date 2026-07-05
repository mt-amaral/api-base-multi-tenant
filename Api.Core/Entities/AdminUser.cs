using Api.Core.Entities.Identity;

namespace Api.Core.Entities;

public class AdminUser
{
    public AdminUser() { }

    public AdminUser(long userId, Guid companyId)
    {
        UserId = userId;
        CompanyId = companyId;
    }

    public long Id { get; private set; }

    public long UserId { get; private set; }

    public User User { get; private set; } = null!;

    public Guid CompanyId { get; private set; }

    public Company Company { get; private set; } = null!;
}
