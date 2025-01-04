using AniLens.Core.Models;
using AniLens.Shared;

namespace AniLens.Core.Interfaces;

public interface IMangaListService
{
    public Result<MangaList> CreateList(string title, Visibility visibility,
        IEnumerable<Manga> content, IEnumerable<User> curators);
    public Result<MangaList> GetList(string id);
    public Result<IEnumerable<MangaList>> GetLists(User user);
    public Result<MangaList> UpdateList(MangaList mangaList);
    public Result<MangaList> DeleteList(string id);
}