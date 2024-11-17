using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LibrarySystemApi.Models;

/// <summary>
/// A custom token
/// </summary>
public class Token
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Owner { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }
}