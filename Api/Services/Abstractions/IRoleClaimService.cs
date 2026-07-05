using Api.Dto;
using Api.Dto.RoleClaim;

namespace Api.Services.Abstractions;

public interface IRoleClaimService
{
    Task<(Response<RoleClaimResponseDto?>, short)> GetByRoleIdAsync(long roleId, CancellationToken ct);

    Task<(Response<bool>, short)> UpdateAsync(long roleId, UpdateRoleClaimRequestDto request, CancellationToken ct);
}
