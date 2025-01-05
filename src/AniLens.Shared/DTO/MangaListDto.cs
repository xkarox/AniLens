namespace AniLens.Shared.DTO;

public class MangaListDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public Visibility Visibility { get; set; } = Visibility.Public;
    public IEnumerable<MangaDto> Content { get; set; } = new List<MangaDto>();
    public IEnumerable<UserDto> Curators { get; set; } = new List<UserDto>();
    public IEnumerable<UserDto> Subscriber { get; set; }  = new List<UserDto>();
    public UserDto? Owner { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}