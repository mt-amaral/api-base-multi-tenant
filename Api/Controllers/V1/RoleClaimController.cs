using Api.Configurations.Identity;
using Api.Dto.RoleClaim;
using Api.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.V1;

[Route("api/v1/roles/{id:long}/claims")]
public class RoleClaimController(IRoleClaimService roleClaimService) : BaseController
{
    [HttpGet]
    [Authorize(Policy = Permissions.ClaimsView)]
    public async Task<IActionResult> GetByRoleId(long id, CancellationToken ct)
    {
        var (data, status) = await roleClaimService.GetByRoleIdAsync(id, ct);
        return StatusCode(status, data);
    }

    [HttpPut]
    [Authorize(Policy = Permissions.ClaimsUpdate)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateRoleClaimRequestDto request, CancellationToken ct)
    {
        var (data, status) = await roleClaimService.UpdateAsync(id, request, ct);
        return StatusCode(status, data);
    }
}
