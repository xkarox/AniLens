using AniLens.Core.Models;
using AniLens.Shared;
using AniLens.Shared.DTO;

namespace AniLens.Core.Interfaces;

public interface IMangaInfoService
{
    public Task<Result<IEnumerable<MangaDto>>> Search(string query);
    public Task<Result<IEnumerable<MangaDto>>> Get(int page, int pageSize);
}