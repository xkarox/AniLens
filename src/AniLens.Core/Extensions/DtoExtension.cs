using AniLens.Core.Models;
using AniLens.Shared.DTO;

namespace AniLens.Core.Extensions;

public static class DtoExtension
{
    public static User ToUser(this UserDto userDto)
    {
        ArgumentNullException.ThrowIfNull(userDto);
        return new User(
            userDto.Id,
            userDto.Username,
            null,
            userDto.Email,
            userDto.CreatedAt,
            userDto.UpdatedAt,
            userDto.Roles);
    }
}