using AniLens.Core.Models;
using AniLens.Shared.DTO;

namespace AniLens.Core.Extensions;

public static class MangaListExtension
{
    public static MangaListDto ToDto(this MangaList mangaList)
    {
        ArgumentNullException.ThrowIfNull(mangaList);
        return new MangaListDto
        {
            Id = mangaList.Id,
            Title = mangaList.Title,
            Visibility = mangaList.Visibility,
            Content = mangaList.Content.Select(manga => manga.ToDto()),
            Curators = mangaList.Curators.Select(curator => curator.ToDto()),
            Subscriber = mangaList.Subscriber.Select(subscriber => subscriber.ToDto()),
            Owner = mangaList.Owner?.ToDto(),
            CreatedAt = mangaList.CreatedAt,
            UpdatedAt = mangaList.UpdatedAt
        };
    }
}