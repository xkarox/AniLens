using AniLens.Shared;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AniLens.Core.Models;

public class MangaList
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public Visibility Visibility { get; set; } = Visibility.Public;
    public IEnumerable<Manga> Content = new List<Manga>();
    public IEnumerable<User> Curators = new List<User>();
    public IEnumerable<User> Subscribers = new List<User>();
}