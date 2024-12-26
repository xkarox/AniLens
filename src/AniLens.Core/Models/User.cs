using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AniLens.Core.Models;

public class User(string? id, string? username, string? password, string? email)
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = id;

    public string? Username { get; set; } = username!;

    public string? Password { get; set; } = password!;

    public string? Email { get; set; } = email;
}