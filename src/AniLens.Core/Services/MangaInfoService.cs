using System.Text;
using AniLens.Core.Extensions;
using AniLens.Core.Interfaces;
using AniLens.Shared;
using AniLens.Shared.DTO;
using MangaDexSharp;
using Microsoft.Extensions.Configuration;
using Manga = AniLens.Core.Models.Manga;

namespace AniLens.Core.Services;

public class MangaInfoService(IMangaDex mangaDexService) : IMangaInfoService
{
    private readonly IMangaDex _mangaDexService = mangaDexService;

    public async Task<Result<IEnumerable<MangaDto>>> Search(string query)
    {
        MangaIncludes[] includes = [MangaIncludes.manga, MangaIncludes.cover_art];
        var result = await mangaDexService.Manga.List(
            new MangaFilter
            {
                Title = query,
                Limit = 100,
                Offset = 0,
            });
        if(result.ErrorOccurred)
            return Result<IEnumerable<MangaDto>>.Failure("MangaDex related Error occurred", Error.Internal);
        var data = result.Data;
        var mangaResult = new List<MangaDto>();
        data.ForEach(entry =>
        {
            var tmp = new Manga
            {
                Id = entry.Id,
                Title = entry.Attributes.Title["en"],
                OriginalLanguage = entry.Attributes.OriginalLanguage,
                LastVolume = entry.Attributes.LastVolume,
                LastChapter = entry.Attributes.LastChapter,
                Status = entry.Attributes.Status.ToString() ?? string.Empty,
                Year = entry.Attributes.Year ?? 0
                
            };
            if (entry.Attributes.Description.TryGetValue("en", out var desc))
                tmp.Description = desc;
            
            var coverFileName =  entry.Relationships
                .Where(relationship => relationship is CoverArtRelationship)
                .Select(relationship => ((CoverArtRelationship)relationship).Attributes.FileName)
                .FirstOrDefault();

            tmp.CoverUri =
                new StringBuilder("https://uploads.mangadex.org/covers/")
                    .Append(entry.Id).Append("/")
                    .Append(coverFileName).ToString();
            
            mangaResult.Add(tmp.ToDto());
        });
        
        return Result<IEnumerable<MangaDto>>.Success(mangaResult);
    }

    public Task<Result<IEnumerable<MangaDto>>> Get(int page, int pageSize)
    {
        throw new NotImplementedException();
    }
}