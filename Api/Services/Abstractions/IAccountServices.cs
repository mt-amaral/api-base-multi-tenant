using Api.Dto;
using Api.Dto.Account;

namespace Api.Services.Abstractions;

public interface IAccountServices
{
    Task<(Response<LoginResponseDto?>, short)> LoginAsync(LoginRequestDto request, CancellationToken ct);
    Task<(Response<string?>, short)> RefreshTokenAsync(CancellationToken ct);
    Task<(Response<string?>, short)> LogoutAsync(CancellationToken ct);
    Task<(Response<LoginResponseDto?>, short)> CheckMe(CancellationToken ct);
}