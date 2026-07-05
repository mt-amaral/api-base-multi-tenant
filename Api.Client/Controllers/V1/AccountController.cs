using Api.Client.Dto.Account;
using Api.Client.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Client.Controllers.V1;

[Route("api/v1")]
public class AccountController(IAccountService accountService) : BaseController
{
    [HttpPost("sessions")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken ct)
    {
        var (data, status) = await accountService.LoginAsync(request, ct);
        return StatusCode(status, data);
    }

    [HttpPost("refresh-tokens")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        var (data, status) = await accountService.RefreshTokenAsync(ct);
        return StatusCode(status, data);
    }

    [HttpGet("sessions/current")]
    public async Task<IActionResult> CheckMe(CancellationToken ct)
    {
        var (data, status) = await accountService.CheckMeAsync(ct);
        return StatusCode(status, data);
    }

    [HttpDelete("sessions/current")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var (data, status) = await accountService.LogoutAsync(ct);
        return StatusCode(status, data);
    }
}
