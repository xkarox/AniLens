using AniLens.Core.Models;
using AniLens.Shared.DTO;
using Mapster;

namespace AniLens.Core.Extensions;

public static class MangaExtension
{
    public static MangaDto ToDto(this Manga manga)
    {
        ArgumentNullException.ThrowIfNull(manga);
        return manga.Adapt<MangaDto>();
    }
}