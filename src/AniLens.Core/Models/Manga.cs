namespace AniLens.Core.Models;

public class Manga
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; }  = string.Empty;
    public string OriginalLanguage { get; set; } = string.Empty;
    public string LastVolume { get; set; } = string.Empty;
    public string LastChapter { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Year { get; set; }
    public string CoverUri { get; set; } = string.Empty;
}

