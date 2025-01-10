using AniLens.Core.Models;
using AniLens.Shared;
using AniLens.Shared.DTO;

namespace AniLens.Core.Interfaces;

public interface IMangaListService
{
    public Task<Result<MangaListDto>> CreateList(MangaListDto item);
    public Task<Result<MangaListDto>> GetList(string id);
    public Task<Result<IEnumerable<MangaListDto>>> Query(MangaListQueryDto query, int page, int pageSize);
    public Task<Result<MangaListDto>> UpdateList(MangaListDto mangaList);
    public Task<Result<MangaListDto>> DeleteList(string id);
}