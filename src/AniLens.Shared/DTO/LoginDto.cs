namespace AniLens.Shared.DTO;

public class LoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public IEnumerable<UserRole> Roles { get; set; } = Enumerable.Empty<UserRole>();
}