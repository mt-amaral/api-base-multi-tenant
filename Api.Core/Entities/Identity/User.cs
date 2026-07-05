using Microsoft.AspNetCore.Identity;

namespace Api.Core.Entities.Identity;

public class User : IdentityUser<long>
{
    public UserType UserType { get; private set; } = UserType.Admin;

    public User() { }

    public User(string userName, string email, UserType userType = UserType.Admin)
    {
        UserName = userName;
        Email = email;
        UserType = userType;
    }

    public void SetUserType(UserType userType)
    {
        UserType = userType;
    }

}
