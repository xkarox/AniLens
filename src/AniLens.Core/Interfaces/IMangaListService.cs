using AniLens.Core.Models;
using AniLens.Shared;
using AniLens.Shared.DTO;

namespace AniLens.Core.Interfaces;

public interface IMangaListService
{
    public Task<Result<MangaListDto>> CreateList(string title, Visibility visibility,
        IEnumerable<MangaDto> content, IEnumerable<UserDto> curators, 
        IEnumerable<UserDto> subscriber, UserDto owner);
    public Task<Result<MangaListDto>> GetList(string id);
    public Task<Result<IEnumerable<MangaListDto>>> Query(MangaListQueryDto query);
    public Task<Result<IEnumerable<MangaListDto>>> GetLists(UserDto user);
    public Task<Result<MangaListDto>> UpdateList(MangaListDto mangaList);
    public Task<Result<MangaListDto>> DeleteList(string id);
}