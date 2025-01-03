using System.Security.Claims;
using AniLens.Core.Models;
using AniLens.Shared;
using AniLens.Shared.DTO;

namespace AniLens.Core.Interfaces;

public interface IJwtService
{
    Result<AuthResponseDto> GenerateToken(LoginDto user);
    // Result<ClaimsPrincipal> ValidateToken(string token);
}