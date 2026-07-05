using Api.Configurations.Identity;
using Api.Dto.User;
using Api.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.V1;

[Route("api/v1/users")]
public class UserController(IUserService userService) : BaseController
{

    [HttpGet]
    [Authorize(Policy = Permissions.UsersView)]
    public async Task<IActionResult> ListUsers([FromQuery] FilterUsersRequestDto request, CancellationToken ct)
    {
        var (data, status) = await userService.GetUsersAsync(request, ct);
        return StatusCode(status, data);
    }

    [HttpPost]
    [Authorize(Policy = Permissions.UsersRegister)]
    public async Task<IActionResult> Create([FromBody] CreateRequestDto request, CancellationToken ct)
    {
        var (data, status) = await userService.CreateAsync(request, ct);
        return StatusCode(status, data);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = Permissions.UsersUpdate)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateUserRequestDto userRequest, CancellationToken ct)
    {
        var (data, status) = await userService.UpdateAsnc(id, userRequest, ct);
        return StatusCode(status, data);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateUserLogged([FromBody] UpdateUserRequestDto userRequest, CancellationToken ct)
    {
        var (data, status) = await userService.UpdateLoggedAsnc(userRequest, ct);
        return StatusCode(status, data);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = Permissions.UsersDelete)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var (data, status) = await userService.DeleteAsync(id, ct);
        return StatusCode(status, data);
    }
}
