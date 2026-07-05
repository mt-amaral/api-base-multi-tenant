using Api.Dto;
using Api.Dto.Role;

namespace Api.Services.Abstractions;

public interface IRoleService
{
    Task<(PagedResponse<List<GetAllRoleResponseDto>?>, short)> ListRolesPageAsync(FilterRoleRequestDto request, CancellationToken ct);

    Task<(Response<List<RoleResponseDto>?>, short)> ListAllRolesAsync(CancellationToken ct);

    Task<(Response<RoleResponseDto?>, short)> CreateAsync(
        CreateRoleRequestDto request,
        CancellationToken ct);

    Task<(Response<RoleResponseDto?>, short)> UpdateAsync(
        long id,
        UpdateRoleRequestDto request,
        CancellationToken ct);

    Task<(Response<bool>, short)> DeleteAsync(
        long id,
        CancellationToken ct);
}
