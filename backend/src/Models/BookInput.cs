namespace LibrarySystemApi.Models;

public class BookInput
{
    public string Barcode { get; set; } = null!;

    public DateTime InputTime { get; set; }

    public string? InputUser { get; set; }

    public StocktakeResponse ReturnedResponse { get; set; } = null!;
}