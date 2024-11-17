using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LibrarySystemApi.Models;

/// <summary>
/// A class storing account information
/// </summary>
public class Account
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool IsAdmin { get; set; }

    public DateTime CreationTime { get; set; }
}