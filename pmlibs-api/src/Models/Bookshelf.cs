using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LibrarySystemApi.Models;

/// <summary>
/// A class storing a single slot of bookshelf
/// </summary>
public class Bookshelf
{
    /// <summary>
    /// Unique Identifier for MongoDB
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>
    /// Represents the session that this shelf belongs to (aka grandparent of this shelf)
    /// </summary>
    public string SessionId { get; set; } = null!;

    /// <summary>
    /// Represents the bookshelf group that this shelf belongs to (aka parent of this shelf)
    /// </summary>
    public string GroupName { get; set; } = null!;

    /// <summary>
    /// Represents the number that can be uniquely identified in parent bookshelf group
    /// </summary>
    public int ShelfNumber { get; set; }

    /// <summary>
    /// Describes the content of the bookshelf (optional)
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Represents the starting time of stock taking this shelf
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Represents the ending time of stock taking this shelf
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Represents if the shelf is unfinished, stock-taking, or finished
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Storing the barcodes (in terms of BookInput) of the books in this shelf
    /// </summary>
    public List<BookInput> AllBooks { get; set; } = null!;
}