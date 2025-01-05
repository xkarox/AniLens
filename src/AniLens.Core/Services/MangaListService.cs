using System.Diagnostics;
using AniLens.Core.Extensions;
using AniLens.Core.Interfaces;
using AniLens.Core.Models;
using AniLens.Core.Settings;
using AniLens.Shared;
using AniLens.Shared.DTO;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AniLens.Core.Services;

public class MangaListService : IMangaListService
{
    private readonly IMongoCollection<MangaList> _mangaListCollection;
    
    public MangaListService(IOptions<MangaListDbSettings> mangaListDbSettings)
    {
        Debug.Assert(mangaListDbSettings != null, nameof(mangaListDbSettings) + " != null");
        var mongoSettings = mangaListDbSettings.Value;
        var client = new MongoClient(mongoSettings.ConnectionUri);
        var database = client.GetDatabase(mongoSettings.DatabaseName);
        _mangaListCollection =
            database.GetCollection<MangaList>(mongoSettings.CollectionName);
    }
    
    public async Task<Result<MangaListDto>> CreateList(string title, Visibility visibility, IEnumerable<MangaDto> content,
        IEnumerable<UserDto> curators, IEnumerable<UserDto> subscriber, UserDto owner)
    {
        try
        {
            var mangaList = new MangaList()
            {
                Title = string.IsNullOrEmpty(title) ? title : "New List",
                Visibility = visibility,
                Content = content.Select(manga => manga.ToManga()),
                Curators = curators.Select(user => user.ToUser()),
                Subscriber = subscriber.Select(user => user.ToUser()),
                Owner = owner.ToUser()
            };
            await _mangaListCollection.InsertOneAsync(mangaList);
            return Result<MangaListDto>.Success(mangaList.ToDto());
        }
        catch
        {
            return Result<MangaListDto>.Failure("Failed to create new list",
                Error.Internal);
        }
    }

    public async Task<Result<MangaListDto>> GetList(string id)
    {
        try
        {
            if(string.IsNullOrEmpty(id))
                return Result<MangaListDto>.Failure("Invalid Id provided", Error.Parameter);
            var list = await _mangaListCollection.Find(mangaList => mangaList.Id == id).FirstOrDefaultAsync();

            return list != null
                ? Result<MangaListDto>.Success(list.ToDto())
                : Result<MangaListDto>.Failure($"List with ID {id} not found",
                    Error.NotFound);
        }
        catch (Exception ex)
        {
            return Result<MangaListDto>.Failure(
                $"Failed to retrieve list: {ex.Message}",
                Error.Internal);
        }
    }

    private FilterDefinition<MangaList> CreateFilter(
        MangaListQueryDto query)
    {
        var builder = Builders<MangaList>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrEmpty(query.Id))
            filter &= builder.Eq(x => x.Id, query.Id);

        if (!string.IsNullOrEmpty(query.Title))
            filter &= builder.Regex(x => x.Title, new BsonRegularExpression(query.Title, "i"));

        if (query.Visibility.HasValue)
            filter &= builder.Eq(x => x.Visibility, query.Visibility.Value);

        if (query.Owner != null)
            filter &= builder.Eq(x => x.Owner.Id, query.Owner.Id);

        if (query.CreatedAtStart.HasValue)
            filter &= builder.Gte(x => x.CreatedAt, query.CreatedAtStart.Value);
        
        if (query.CreatedAtEnd.HasValue)
            filter &= builder.Lte(x => x.CreatedAt, query.CreatedAtEnd.Value);

        if (query.UpdatedAtStart.HasValue)
            filter &= builder.Gte(x => x.UpdatedAt, query.UpdatedAtStart.Value);
        
        if (query.UpdatedAtEnd.HasValue)
            filter &= builder.Gte(x => x.UpdatedAt, query.UpdatedAtEnd.Value);

        if (query.Content?.Any() == true)
        {
            var mangaIds = query.Content.Select(manga => manga.Id);
            filter &= builder.ElemMatch(x => x.Content,
                manga => mangaIds.Contains(manga.Id));
        }
        
        if (query.Curators?.Any() == true)
        {
            var curatorIds = query.Curators.Select(c => c.Id);
            filter &= builder.ElemMatch(x => x.Curators,
                curator => curatorIds.Contains(curator.Id));
        }
        
        if (query.Subscriber?.Any() == true)
        {
            var subscriberIds = query.Subscriber.Select(c => c.Id);
            filter &= builder.ElemMatch(x => x.Subscriber,
                subscriber => subscriberIds.Contains(subscriber.Id));
        }

        return filter;
    }

    public async Task<Result<IEnumerable<MangaListDto>>> Query(MangaListQueryDto query)
    {
        try
        {
            var filter = CreateFilter(query);
            var result = await _mangaListCollection.Find(filter).ToListAsync();

            return result != null
                ? Result<IEnumerable<MangaListDto>>.Success(
                    result.Select(list => list.ToDto()))
                : Result<IEnumerable<MangaListDto>>.Failure(
                    "Failed to find any lists", Error.NotFound);
        }
        catch (Exception e)
        {
            return Result<IEnumerable<MangaListDto>>.Failure(
                "Failed to run query", Error.Internal);
        }
    }

    public async Task<Result<IEnumerable<MangaListDto>>> GetLists(UserDto user)
    {
        try
        {
            var list = await _mangaListCollection.Find(mangaList => mangaList.Owner!.Id == user.Id).ToListAsync();

            return list != null
                ? Result<IEnumerable<MangaListDto>>.Success(list.Select(mangaList => mangaList.ToDto()))
                : Result<IEnumerable<MangaListDto>>.Failure(
                    $"No Lists found for user {user.Username} with ID {user.Id}",
                    Error.NotFound);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<MangaListDto>>.Failure(
                $"Failed to retrieve list: {ex.Message}",
                Error.Internal);
        }
    }

    public async Task<Result<MangaListDto>> UpdateList(MangaListDto updatelist)
    {
        try
        {
            var filter = Builders<MangaList>.Filter.Eq("_id", updatelist.Id);
            var update = Builders<MangaList>.Update
                .Set(x => x.Title, updatelist.Title)
                .Set(x => x.Visibility, updatelist.Visibility)
                .Set(x => x.Content, updatelist.Content.Select(manga => manga.ToManga()))
                .Set(x => x.Curators, updatelist.Curators.Select(user => user.ToUser()))
                .Set(x => x.Subscriber, updatelist.Subscriber.Select(user => user.ToUser()))
                .Set(x => x.Owner, updatelist.Owner!.ToUser())
                .Set(x => x.UpdatedAt, DateTime.UtcNow);
            var options = new FindOneAndUpdateOptions<MangaList> 
            {
                ReturnDocument = ReturnDocument.After
            };
            var result =
                await _mangaListCollection.FindOneAndUpdateAsync(filter, update,
                    options);
            return result != null
                ? Result<MangaListDto>.Success(result.ToDto())
                : Result<MangaListDto>.Failure($"Failed to update list with ID {updatelist.Id}. List not found",
                    Error.NotFound);
        }
        catch (Exception ex)
        {
            return Result<MangaListDto>.Failure(
                $"Failed to update list: {ex.Message}",
                Error.Internal);
        }
    }

    public async Task<Result<MangaListDto>> DeleteList(string id)
    {
        try
        {
            var filter = Builders<MangaList>.Filter.Eq("_id", id);
            var options = new FindOneAndDeleteOptions<MangaList>();
            var result =
                await _mangaListCollection.FindOneAndDeleteAsync(filter, options);

            return result != null
                ? Result<MangaListDto>.Success(result.ToDto())
                : Result<MangaListDto>.Failure(
                    $"Failed to delete list with ID {id}. List not found",
                    Error.NotFound);
        }
        catch (Exception ex)
        {
            return Result<MangaListDto>.Failure("Failed to delete list",
                Error.Internal);
        }
    }
}