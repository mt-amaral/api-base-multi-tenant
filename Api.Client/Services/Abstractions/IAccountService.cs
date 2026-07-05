using Api.Client.Dto;
using Api.Client.Dto.Account;

namespace Api.Client.Services.Abstractions;

public interface IAccountService
{
    Task<(Response<LoginResponseDto?>, short)> LoginAsync(LoginRequestDto request, CancellationToken ct);
    Task<(Response<string?>, short)> RefreshTokenAsync(CancellationToken ct);
    Task<(Response<string?>, short)> LogoutAsync(CancellationToken ct);
    Task<(Response<LoginResponseDto?>, short)> CheckMeAsync(CancellationToken ct);
}
