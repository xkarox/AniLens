using AniLens.Shared.DTO.Base;

namespace AniLens.Shared.DTO;

public class UserDto : IUserDto
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IEnumerable<UserRole> Roles { get; set; } = Enumerable.Empty<UserRole>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}