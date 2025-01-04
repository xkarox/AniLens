using AniLens.Core.Extensions;
using AniLens.Core.Models;
using AniLens.Shared;
using AniLens.Shared.DTO;

namespace AniLens.Core.Test.Extension;

public class UserExtensionTest
{
    private const string Id = "some_random_id_doesnt";
    private const string Username = "username";
    private const string PasswordHash = "lksadjflksajdfl;kjsadf";
    private const string Email = "definetlyLegit@email.com";
    private static readonly DateTime CreatedAt = DateTime.UtcNow.AddMonths(-1);
    private static readonly DateTime UpdatedAt = DateTime.UtcNow.AddHours(-1);
    private static readonly IEnumerable<UserRole> Roles = new List<UserRole> { UserRole.User };
    
    private readonly User _user = new User(Id, Username, PasswordHash, Email, CreatedAt,
        UpdatedAt, Roles);
    
    [Fact]
    public void ToDto()
    {
        var userDto = _user.ToDto();

        Assert.IsType<UserDto>(userDto);
        Assert.Equal(Id, userDto.Id);
        Assert.Equal(Id, userDto.Id);
        Assert.Equal(Username, userDto.Username);
        Assert.Equal(Email, userDto.Email);
        Assert.Equal(CreatedAt, userDto.CreatedAt);
        Assert.Equal(UpdatedAt, userDto.UpdatedAt);
        Assert.Equal(Roles, userDto.Roles);
    }
    
    [Fact]
    public void ToDto_NullArguments()
    {
        var user = new User(Id, null, PasswordHash, null, CreatedAt, UpdatedAt,
            null);
        var userDto = user.ToDto();
        
        Assert.IsType<UserDto>(userDto);
        Assert.Equal(Id, userDto.Id);
        Assert.Equal(Id, userDto.Id);
        Assert.Equal(string.Empty, userDto.Username);
        Assert.Equal(string.Empty, userDto.Email);
        Assert.Equal(CreatedAt, userDto.CreatedAt);
        Assert.Equal(UpdatedAt, userDto.UpdatedAt);
        Assert.Equal(new List<UserRole>(), userDto.Roles);
    }
    
    [Fact]
    public void ToDto_NullUser_ThrowsArgumentNullException()
    {
        User nullUser = null;

        Assert.Throws<ArgumentNullException>(() => nullUser.ToDto());
    }

    [Fact]
    public void ToDto_Collection()
    {
        var users = new List<User>()
        {
            _user,
            _user,
            _user
        };

        var userDtos = users.ToDto();
        
        Assert.IsAssignableFrom<IEnumerable<UserDto>>(userDtos);
        Assert.Equal(3, userDtos.Count());
    }
    
    [Fact]
    public void ToDto_EmptyCollection()
    {
        var users = new List<User>();
        var userDtos = users.ToDto();
    
        Assert.IsAssignableFrom<IEnumerable<UserDto>>(userDtos);
        Assert.Empty(userDtos);
    }
    
    [Fact]
    public void ToDto_NullUserCollection_ThrowsArgumentNullException()
    {
        IEnumerable<User> nullUsers = null;

        Assert.Throws<ArgumentNullException>(() => nullUsers.ToDto());
    }

    [Fact]
    public void ToLogin()
    {
        var loginDto = _user.ToLogin();

        Assert.IsType<LoginDto>(loginDto);
        Assert.Equal(Username, loginDto.Username);
        Assert.Equal(PasswordHash, loginDto.Password);
        Assert.Equal(Roles, loginDto.Roles);
    }
    
    [Fact]
    public void ToLogin_NullArgument()
    {
        var user = new User(Id, Username, null, Email, CreatedAt, UpdatedAt, Roles);
        var loginDto = user.ToLogin();

        Assert.IsType<LoginDto>(loginDto);
        Assert.Equal(Username, loginDto.Username);
        Assert.Equal(string.Empty, loginDto.Password);
        Assert.Equal(Roles, loginDto.Roles);
    }
    
    [Fact]
    public void ToLogin_NullUser_ThrowsArgumentNullException()
    {
        User nullUser = null;

        Assert.Throws<ArgumentNullException>(() => nullUser.ToLogin());
    }
}
