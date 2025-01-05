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
    public IEnumerable<Manga> Content { get; set; } = new List<Manga>();
    public IEnumerable<User> Curators { get; set; } = new List<User>();
    public IEnumerable<User> Subscriber { get; set; }  = new List<User>();
    public User? Owner { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}