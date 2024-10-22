namespace LibrarySystemApi.Models;

public class StocktakePayload
{
    public string SessionId { get; set; } = null!;

    public string BookshelfId { get; set; } = null!;

    public string Operation { get; set; } = null!;

    public string? Barcode { get; set; }
}