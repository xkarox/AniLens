using AniLens.Core.Interfaces;
using AniLens.Core.Services;

namespace AniLens.Core.Test.Services;

public class HashService_Test
{
    private readonly IHashService _hashService = new HashService();
    
    [Fact]
    public void HashPassword_ValidPassword_ReturnsSuccessResult()
    {
        var result = _hashService.HashPassword("validPassword123");
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public void HashPassword_NullPassword_ThrowsArgumentNullException()
    {
        var result = _hashService.HashPassword(null);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void HashPassword_EmptyPassword_ReturnsFailureResult()
    {
        var result = _hashService.HashPassword("");
        Assert.True(result.IsFailure);
    }
    
    [Fact]
    public void HashPassword_WhitespacePassword_ReturnsFailureResult()
    {
        var result = _hashService.HashPassword("   ");
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void HashPassword_LongPassword_ReturnsSuccessResult()
    {
        var longPassword = new string('a', 1000);
        var result = _hashService.HashPassword(longPassword);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void CheckPassword_CorrectPassword_ReturnsTrue()
    {
        var password = "correctPassword123";
        var hashedPassword = _hashService.HashPassword(password).Data;
        Assert.True(_hashService.CheckPassword(password, hashedPassword));
    }

    [Fact]
    public void CheckPassword_IncorrectPassword_ReturnsFalse()
    {
        var password = "correctPassword123";
        var hashedPassword = _hashService.HashPassword(password).Data;
        Assert.False(_hashService.CheckPassword("wrongPassword", hashedPassword));
    }

    [Fact]
    public void CheckPassword_NullPassword_ReturnsFalse()
    {
        Assert.False(_hashService.CheckPassword(null, "hashedPassword"));
    }

    [Fact]
    public void CheckPassword_NullHashedPassword_ReturnsFalse()
    {
        Assert.False(_hashService.CheckPassword("password", null));
    }

    [Fact]
    public void CheckPassword_EmptyPassword_ReturnsFalse()
    {
        var hashedPassword = _hashService.HashPassword("somePassword").Data;
        Assert.False(_hashService.CheckPassword("", hashedPassword));
    }

    [Fact]
    public void CheckPassword_InvalidHashFormat_WrongLength_ReturnsFalse()
    {
        Assert.False(_hashService.CheckPassword("password", "invalidHash"));
    }
    
    [Fact]
    public void CheckPassword_InvalidHashFormat_Empty_ReturnsFalse()
    {
        Assert.False(_hashService.CheckPassword("password", ""));
    }
    
    [Fact]
    public void CheckPassword_InvalidHashFormat_Null_ReturnsFalse_2()
    {
        Assert.False(_hashService.CheckPassword("password", null));
    }
    
    [Fact]
    public void CheckPassword_InvalidPasswordFormat_Null_ReturnsFalse()
    {
        Assert.False(_hashService.CheckPassword(null, "invalidHash"));
    }
    
    [Fact]
    public void CheckPassword_InvalidPasswordFormat_Empty_ReturnsFalse()
    {
        Assert.False(_hashService.CheckPassword("", "invalidHash"));
    }
}