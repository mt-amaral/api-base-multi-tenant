using Api.Dto.Account;
using Api.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.V1;


[Route("api/v1")]
public class AccountController(IAccountServices accountServices) : BaseController
{


    [HttpPost("sessions")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken ct)
    {
        var (data, status) = await accountServices.LoginAsync(request, ct);
        return StatusCode(status, data);
    }

    [HttpPost("refresh-tokens")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        var (data, status) = await accountServices.RefreshTokenAsync(ct);
        return StatusCode(status, data);
    }

    [HttpGet("sessions/current")]
    public async Task<IActionResult> CheckMe(CancellationToken ct)
    {
        var (data, status) = await accountServices.CheckMe(ct);
        return StatusCode(status, data);
    }

    [HttpDelete("sessions/current")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var (data, status) = await accountServices.LogoutAsync(ct);
        return StatusCode(status, data);
    }
}
