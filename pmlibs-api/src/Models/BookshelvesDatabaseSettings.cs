namespace LibrarySystemApi.Models;

public class BookshelvesDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string BookshelvesCollectionName { get; set; } = null!;
}