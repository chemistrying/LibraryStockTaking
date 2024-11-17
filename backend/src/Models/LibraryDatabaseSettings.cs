namespace LibrarySystemApi.Models;

public class LibraryDatabaseSettings
{
    public static string ConnectionString { get; set; } = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "mongodb://localhost:27017/";

    public static string DatabaseName { get; set; } = Environment.GetEnvironmentVariable("CONNECTION_DATABASE") ?? "library";
}