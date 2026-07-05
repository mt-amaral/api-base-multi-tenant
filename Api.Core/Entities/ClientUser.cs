using Api.Core.Entities.Identity;

namespace Api.Core.Entities;

public class ClientUser
{
    public ClientUser() { }

    public ClientUser(long userId, Guid companyId)
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
