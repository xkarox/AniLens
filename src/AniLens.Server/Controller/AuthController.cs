using System.Runtime.InteropServices.JavaScript;
using AniLens.Core.Extensions;
using AniLens.Core.Interfaces;
using AniLens.Core.Services;
using AniLens.Shared;
using AniLens.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AniLens.Server.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController(JwtService jwtService, UserService userService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        var userFetchResult = await userService.GetByName(loginDto.Username);
        if (userFetchResult.IsSuccess)
        {
            var user = userFetchResult.Data!;
            if (user.PasswordHash == loginDto.Password)
            {
                var tokenGenResult = jwtService.GenerateToken(loginDto);
                if (tokenGenResult.IsSuccess)
                {
                    tokenGenResult.Data!.User = user.ToDto();
                    return Ok(tokenGenResult.Data);
                }

                return StatusCode(500, Error.Internal.ToDescriptionString());
            }
            return Unauthorized(Error.InvalidCredentials.ToDescriptionString());
        }
        return BadRequest(Error.InvalidCredentials.ToDescriptionString());
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
    {
        var result = await userService.AddUser(registerDto);
        if (result.IsSuccess)
        {
            var tokenGenResult =
                jwtService.GenerateToken(
                    result.Data!.ToLogin(registerDto.Password));

            if (tokenGenResult.IsSuccess)
            {
                tokenGenResult.Data!.User = result.Data!;
                return Ok(tokenGenResult.Data);
            }
            
            return StatusCode(500, Error.Internal.ToDescriptionString());
        }

        return BadRequest(Error.Parameter);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken()
    {
        throw new NotImplementedException();
    }
}