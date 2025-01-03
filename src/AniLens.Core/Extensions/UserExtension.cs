using AniLens.Core.Models;
using AniLens.Shared;
using AniLens.Shared.DTO;

namespace AniLens.Core.Extensions;

public static class UserExtension
{
    public static UserDto ToDto(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new UserDto()
        {
            Id = user.Id,
            Username = user.Username ?? "",
            Email = user.Email ?? "",
            Roles = user.Roles ?? new List<UserRole>(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
    public static IEnumerable<UserDto> ToDto(this IEnumerable<User> users)
    {
        ArgumentNullException.ThrowIfNull(users);

        return users.Select(user => user.ToDto());
    }

    public static LoginDto ToLogin(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        return user.ToDto().ToLogin(user.PasswordHash ?? string.Empty);
    }
}
