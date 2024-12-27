using AniLens.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AniLens.Server.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        throw new NotImplementedException();
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
    {
        throw new NotImplementedException();
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken()
    {
        throw new NotImplementedException();
    }
}