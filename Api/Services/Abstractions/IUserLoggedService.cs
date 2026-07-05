

using Api.Core.Entities.Identity;

namespace Api.Services.Abstractions
{
    public interface IUserLoggedService
    {
        Task<User> GetUserLoggedAsync();
    }
}