using AniLens.Core.Extensions;
using AniLens.Core.Models;
using AniLens.Shared;
using AniLens.Shared.DTO;

namespace AniLens.Core.Test.Extension;

public class DtoExtension_Test
{
    private const string Id = "some_random_id_doesnt";
    private const string Username = "username";
    private const string Password = "lksadjflksajdfl;kjsadf";
    private const string Email = "definetlyLegit@email.com";
    private static readonly DateTime CreatedAt = DateTime.UtcNow.AddMonths(-1);
    private static readonly DateTime UpdatedAt = DateTime.UtcNow.AddHours(-1);
    private static readonly IEnumerable<UserRole> Roles = new List<UserRole> { UserRole.User };
    
    private readonly UserDto _userDto = new UserDto()
    {
        Id = Id, 
        Username = Username, 
        Email = Email, 
        CreatedAt = CreatedAt,
        UpdatedAt = UpdatedAt, 
        Roles = Roles
    };
    
    private readonly RegisterDto _registerDto = new RegisterDto()
    {
        Username = Username,
        Password = Password,
        Email = Email
    };

    [Fact]
    public void ToUser_UserDto()
    {
        var user = _userDto.ToUser();

        Assert.IsType<User>(user);
        Assert.Equal(Id, user.Id);
        Assert.Equal(Username, user.Username);
        Assert.Null(user.PasswordHash);
        Assert.Equal(Email, user.Email);
        Assert.Equal(CreatedAt, user.CreatedAt);
        Assert.Equal(UpdatedAt, user.UpdatedAt);
        Assert.Equal(Roles, user.Roles);
    }

    [Fact]
    public void ToUser_NullUserDto_ThrowsArgumentNullException()
    {
        UserDto userDto = null;

        Assert.Throws<ArgumentNullException>(() => userDto.ToUser());
    }

    [Fact]
    public void ToUser_RegisterDto()
    {
        var user = _registerDto.ToUser();

        Assert.IsType<User>(user);
        Assert.Null(user.Id);
        Assert.Equal(Username, user.Username);
        Assert.Equal(Password, user.PasswordHash);
        Assert.Equal(Email, user.Email);
        Assert.IsType<DateTime>(user.CreatedAt);
        Assert.IsType<DateTime>(user.UpdatedAt);
        Assert.NotNull(user.Roles);
        Assert.Contains(UserRole.User, user.Roles);
    }

    [Fact]
    public void ToUser_NullRegisterDto_ThrowsArgumentNullException()
    {
        RegisterDto registerDto = null;

        Assert.Throws<ArgumentNullException>(() => registerDto.ToUser());
    }

    [Fact]
    public void ToLogin_UserDto()
    {
        var loginDto = _userDto.ToLogin(Password);

        Assert.IsType<LoginDto>(loginDto);
        Assert.Equal(Username, loginDto.Username);
        Assert.Equal(Password, loginDto.Password);
        Assert.Equal(Roles, loginDto.Roles);
    }

    [Fact]
    public void ToLogin_NullUserDto_ThrowsArgumentNullException()
    {
        UserDto userDto = null;

        Assert.Throws<ArgumentNullException>(() => userDto.ToLogin(Password));
    }
}