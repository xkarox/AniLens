using AniLens.Shared;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AniLens.Core.Models;

public class User(string? id, string? username, string? passwordHash, string? email, 
    DateTime createdAt, DateTime updatedAt, IEnumerable<UserRole>? roles)
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = id;

    public string? Username { get; set; } = username!;

    public string? PasswordHash { get; set; } = passwordHash!;

    public string? Email { get; set; } = email;

    public DateTime CreatedAt { get; set; } = createdAt;

    public DateTime UpdatedAt { get; set; } = updatedAt;

    public IEnumerable<UserRole>? Roles { get; set; } = roles;
}