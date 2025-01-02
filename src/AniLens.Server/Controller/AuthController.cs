using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using AniLens.Core.Extensions;
using AniLens.Core.Interfaces;
using AniLens.Core.Services;
using AniLens.Shared;
using AniLens.Shared.DTO;
using DevOne.Security.Cryptography.BCrypt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AniLens.Server.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IJwtService jwtService, 
    IUserService userService,
    IHashService hashService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        var userFetchResult = await userService.GetByName(loginDto.Username);
        if (userFetchResult.IsFailure)
            return BadRequest(Error.InvalidCredentials.ToDescriptionString());

        var user = userFetchResult.Data!;
        if(!hashService.CheckPassword(loginDto.Password, user.PasswordHash!))
            return Unauthorized(Error.InvalidCredentials.ToDescriptionString());

        loginDto.Roles = user.Roles ?? [];
        var tokenGenResult = jwtService.GenerateToken(loginDto);
        if (tokenGenResult.IsFailure)
            return StatusCode(500, Error.Internal.ToDescriptionString());
        
        Response.Cookies.Append("jwt", tokenGenResult.Data!.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = tokenGenResult.Data.Expiration
        });
        
        return Ok(user.ToDto());
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
    {
        var hashPasswordResult = hashService.HashPassword(registerDto.Password);
        if (hashPasswordResult.IsFailure)
            return StatusCode(500, Error.Internal.ToDescriptionString());

        registerDto.Password = hashPasswordResult.Data!;
        
        var userFetchResult = await userService.AddUser(registerDto);
        if (userFetchResult.IsFailure)
        {
            return StatusCode(500, Error.Internal.ToDescriptionString());
        }
        
        var tokenGenResult =
            jwtService.GenerateToken(
                userFetchResult.Data!.ToLogin(registerDto.Password));
        if (tokenGenResult.IsFailure)
        {
            return BadRequest(Error.Parameter);
        }
        
        Response.Cookies.Append("jwt", tokenGenResult.Data!.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = tokenGenResult.Data.Expiration
        });
        
        tokenGenResult.Data!.User = userFetchResult.Data!;
        return Ok(tokenGenResult.Data!.User);

    }
    
    
    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken()
    {
        Request.Cookies.TryGetValue("jwt", out var token);
        var validationResult = jwtService.ValidateToken(token!);
        if (validationResult.IsFailure)
        {
            return BadRequest(Error.Unauthorized.ToDescriptionString());
        } 
        var claimsPrincipal = validationResult.Data;
        if (claimsPrincipal == null)
        {
            return StatusCode(500, Error.Internal.ToDescriptionString());
        }
        var username = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
        var userFetchResult = await userService.GetByName(username!.Value);
        if (userFetchResult.IsFailure)
        {
            return StatusCode(500, Error.Internal.ToDescriptionString());
        }
        var user = userFetchResult.Data!.ToLogin();
        var tokenGenResult = jwtService.GenerateToken(user);
        if (tokenGenResult.IsFailure)
            return StatusCode(500, Error.Internal.ToDescriptionString());
        Response.Cookies.Append("jwt", tokenGenResult.Data!.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = tokenGenResult.Data.Expiration
        }); 
        return Ok();
    }
}