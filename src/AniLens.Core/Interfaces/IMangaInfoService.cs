using AniLens.Core.Models;
using AniLens.Shared;

namespace AniLens.Core.Interfaces;

public interface IMangaInfoService
{
    public Task<Result<IEnumerable<Manga>>> Search(string query);
    public Task<Result<IEnumerable<Manga>>> Get(int page, int pageSize);
}