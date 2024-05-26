using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LibrarySystemApi.Models;

/// <summary>
/// A class storing the whole manual stock taking session
/// </summary>
public class StockTakingSession
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string SessionName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public bool IsActive { get; set; }

    public List<BookshelfGroup> AllBookshelfGroups { get; set; } = null!;
}