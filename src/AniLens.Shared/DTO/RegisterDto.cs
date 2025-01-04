using AniLens.Shared.DTO.Base;

namespace AniLens.Shared.DTO;

public class RegisterDto : IUserDto
{
    public string Password { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}