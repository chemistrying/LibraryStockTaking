namespace LibrarySystemApi.Models;

public class SessionsDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string SessionsCollectionName { get; set; } = null!;
}