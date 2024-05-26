using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LibrarySystemApi.Models;

public class Book
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Acno { get; set; } = null!;
    public string Callno1 { get; set; } = null!;
    public string Callno2 { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string Publisher { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Language { get; set; } = null!;
    public string Category { get; set; } = null!;

    public Book(string[] Blocks)
    {
        for (int i = 0; i < 13; i++)
        {
            switch (i)
            {
                case 0:
                    Acno = Blocks[i];
                    break;
                case 2:
                    Callno1 = Blocks[i];
                    break;
                case 4:
                    Callno2 = Blocks[i];
                    break;
                case 5:
                    Name = Blocks[i];
                    break;
                case 6:
                    Status = Blocks[i];
                    break;
                case 7:
                    Publisher = Blocks[i];
                    break;
                case 8:
                    Author = Blocks[i];
                    break;
                case 11:
                    Language = Blocks[i];
                    break;
                case 12:
                    Category = Blocks[i];
                    break;
                default:
                    break;
            }
        }
    }

    public string ToStandardFormat()
    {
        return $"{Acno} | {Callno1} | {Callno2} | {Name} | {Status} | {Publisher} | {Author} | {Language} | {Category}";
    }
}