namespace AniLens.Shared.DTO;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
    public DateTime Expiration { get; set; }
}