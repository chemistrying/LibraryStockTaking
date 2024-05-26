using MongoDB.Bson;

namespace LibrarySystemApi.Models;

/// <summary>
/// A class storing a row / group of bookshelves
/// </summary>
public class BookshelfGroup
{
    /// <summary>
    /// An identifier for naming the bookshelf group
    /// </summary>
    public string GroupName { get; set; } = null!;

    /// <summary>
    /// Represents the bookshelf object ID in string of the bookshelf group
    /// </summary>
    public List<string> AllBookshelvesId { get; set; } = null!;
}