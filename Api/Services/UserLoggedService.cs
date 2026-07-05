using Api.Core.Entities.Identity;
using Api.Services.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Api.Services;

public class UserLoggedService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
    : IUserLoggedService
{
    public async Task<User> GetUserLoggedAsync() =>
        await userManager.GetUserAsync(httpContextAccessor.HttpContext?.User!)
        ?? throw new Exception("Erro ao obter usuario logado!");
}