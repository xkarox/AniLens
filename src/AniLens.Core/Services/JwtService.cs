using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AniLens.Core.Interfaces;
using AniLens.Core.Models;
using AniLens.Shared;
using AniLens.Shared.DTO;
using AniLens.Shared.DTO.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AniLens.Core.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly TokenValidationParameters _validationParameters;
    private readonly IUserService _userService;
    
    public JwtService(IConfiguration configuration, IUserService userService)
    {
        _configuration = configuration;
        _tokenHandler = new JwtSecurityTokenHandler();
        _userService = userService;
        
        var secretKey = _configuration["JwtSettings:SecretKey"];
        if (string.IsNullOrEmpty(secretKey) || Encoding.UTF8.GetBytes(secretKey).Length < 32)
               throw new ArgumentException("JWT secret key must be at least 32 bytes");
        
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey));
            
        _validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["JwtSettings:Issuer"],
            ValidAudience = _configuration["JwtSettings:Audience"],
            IssuerSigningKey = securityKey,
            ClockSkew = TimeSpan.Zero // Optional: removes the default 5 minute clock skew
        };
    }
    public Result<AuthResponseDto> GenerateToken(LoginDto user)
    {
        try
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["JwtSettings:SecretKey"]!));
            var credentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };
            
            claims.AddRange(user.Roles.Select(
                role => new Claim(ClaimTypes.Role, role.ToString())));

            var expiration = DateTime.UtcNow.AddMinutes(
                Convert.ToInt32(
                    _configuration["JwtSettings:ExpirationInMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );
            
            var response = new AuthResponseDto()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
            return Result<AuthResponseDto>.Success(response);
        }
        catch
        {
            return Result<AuthResponseDto>.Failure(
                Error.Internal.ToDescriptionString(),
                Error.Internal);
        }
        
        
    }

    public Result<ClaimsPrincipal> ValidateToken(string token)
    {
        try
        {
            var principal = _tokenHandler.ValidateToken(token, _validationParameters, out var validatedToken);

            if (validatedToken is JwtSecurityToken jwtToken)
            {
                var hasValidSecurityAlgorithm = jwtToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256, 
                    StringComparison.InvariantCultureIgnoreCase);

                if (!hasValidSecurityAlgorithm)
                    return Result<ClaimsPrincipal>.Failure("Invalid token algorithm", Error.Unauthorized);
            }

            return Result<ClaimsPrincipal>.Success(principal);
        }
        catch (SecurityTokenExpiredException)
        {
            return Result<ClaimsPrincipal>.Failure("Token has expired", Error.Unauthorized);
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            return Result<ClaimsPrincipal>.Failure("Token has invalid signature", Error.Unauthorized);
        }
        catch (Exception ex)
        {
            return Result<ClaimsPrincipal>.Failure($"Token validation failed: {ex.Message}", Error.Unauthorized);
        }
    }
}