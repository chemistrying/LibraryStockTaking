namespace LibrarySystemApi.Models;

public class StocktakeResponse
{
    public string Verdict { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? BookInformation { get; set; }
}