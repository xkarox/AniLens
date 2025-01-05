namespace AniLens.Shared.DTO;

public class MangaListQueryDto
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public Visibility? Visibility { get; set; }
    public IEnumerable<MangaDto>? Content { get; set; }
    public IEnumerable<UserDto>? Curators { get; set; }
    public IEnumerable<UserDto>? Subscriber { get; set; }
    public UserDto? Owner { get; set; }
    public DateTime? CreatedAtStart { get; set; }
    public DateTime? CreatedAtEnd { get; set; }
    public DateTime? UpdatedAtStart { get; set; }
    public DateTime? UpdatedAtEnd { get; set; }
}