using AniLens.Core.Models;
using AniLens.Shared;
using AniLens.Shared.DTO;
using AniLens.Shared.DTO.Base;
using Mapster;

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
            userDto.Roles
            );
    }
    
    public static User ToUser(this RegisterDto registerDto)
    {
        ArgumentNullException.ThrowIfNull(registerDto);
        return new User(
            null,
            registerDto.Username,
            registerDto.Password,
            registerDto.Email,
            DateTime.UtcNow, 
            DateTime.UtcNow, 
            [UserRole.User]
        );
    }

    public static LoginDto ToLogin(this UserDto userDto, string password)
    {
        ArgumentNullException.ThrowIfNull(userDto);
        return new LoginDto()
        {
            Username = userDto.Username,
            Password = password,
            Roles = userDto.Roles
        };
    }

    public static Manga ToManga(this MangaDto mangaDto)
    {
        ArgumentNullException.ThrowIfNull(mangaDto);
        return mangaDto.Adapt<Manga>();
    }
}