using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AniLens.Core.Services;
using AniLens.Shared;
using AniLens.Shared.DTO;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Moq.Protected;

namespace AniLens.Core.Test.Services;
using Microsoft.Extensions.Configuration;

public class JwtService_Test
{
    private Mock<IConfiguration> _mockConfiguration;
    private readonly JwtService _jwtService;

    public JwtService_Test()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(c => c["JwtSettings:Issuer"]).Returns("TestIssuer");
        _mockConfiguration.Setup(c => c["JwtSettings:Audience"]).Returns("TestAudience");
        _mockConfiguration.Setup(c => c["JwtSettings:ExpirationInMinutes"]).Returns("5");
        _mockConfiguration.Setup(c => c["JwtSettings:SecretKey"]).Returns("ThisIsAVeryLongSecretKeyForTesting");
        
        
        _jwtService = new JwtService(_mockConfiguration.Object);
    }

    [Fact]
    public void Constructor_ValidConfiguration_CreatesJwtService()
    {
        Assert.NotNull(_jwtService);
    }

    [Fact]
    public void Constructor_MissingExpirationSetting_ThrowsInvalidOperationException()
    {
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["JwtSettings:ExpirationInMinutes"]).Returns((string)null);

        Assert.Throws<InvalidOperationException>(() => new JwtService(mockConfig.Object));
    }

    [Fact]
    public void Constructor_InvalidExpirationSetting_ThrowsArgumentException()
    {
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["JwtSettings:Issuer"]).Returns("TestIssuer");
        mockConfig.Setup(c => c["JwtSettings:Audience"]).Returns("TestAudience");
        mockConfig.Setup(c => c["JwtSettings:ExpirationInMinutes"]).Returns("invalid");
        mockConfig.Setup(c => c["JwtSettings:SecretKey"]).Returns("ThisIsAVeryLongSecretKeyForTesting");


        Assert.Throws<ArgumentException>(() => new JwtService(mockConfig.Object));
    }

    [Fact]
    public void Constructor_NegativeExpirationSetting_ThrowsArgumentException()
    {
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["JwtSettings:Issuer"]).Returns("TestIssuer");
        mockConfig.Setup(c => c["JwtSettings:Audience"]).Returns("TestAudience");
        mockConfig.Setup(c => c["JwtSettings:ExpirationInMinutes"]).Returns("-1");
        mockConfig.Setup(c => c["JwtSettings:SecretKey"]).Returns("ThisIsAVeryLongSecretKeyForTesting");

        Assert.Throws<ArgumentException>(() => new JwtService(mockConfig.Object));
    }

    [Fact]
    public void GenerateToken_ValidUser_ReturnsSuccessResult()
    {
        var user = new LoginDto
        {
            Username = "testuser",
            Roles = new List<UserRole> { UserRole.Admin, UserRole.User }
        };

        var result = _jwtService.GenerateToken(user);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data.Token);
        Assert.True(result.Data.Expiration > DateTime.UtcNow);
    }

    [Fact]
    public void GenerateToken_ValidUser_ContainsExpectedClaims()
    {
        var user = new LoginDto
        {
            Username = "testuser",
            Roles = new List<UserRole> { UserRole.Admin, UserRole.User }
        };

        var result = _jwtService.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(result.Data!.Token) as JwtSecurityToken;

        Assert.NotNull(jsonToken);
        Assert.Contains(jsonToken.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Username);
        Assert.Contains(jsonToken.Claims, c => c.Type == JwtRegisteredClaimNames.Jti);
        Assert.Contains(jsonToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        Assert.Contains(jsonToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == "User");
    }

    [Fact]
    public void GenerateToken_NullUser_ReturnsFailureResult()
    {
        var result = _jwtService.GenerateToken(null);

        Assert.False(result.IsSuccess);
        Assert.Equal(Error.Internal.ToDescriptionString(), result.Error);
    }

    [Fact]
    public void GenerateToken_UserWithNoRoles_GeneratesTokenWithoutRoleClaims()
    {
        var user = new LoginDto
        {
            Username = "testuser",
            Roles = new List<UserRole>()
        };

        var result = _jwtService.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(result.Data!.Token) as JwtSecurityToken;

        Assert.NotNull(jsonToken);
        Assert.DoesNotContain(jsonToken.Claims, c => c.Type == ClaimTypes.Role);
    }
}
