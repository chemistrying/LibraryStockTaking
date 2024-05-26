using LibrarySystemApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LibrarySystemApi.Services;

public class BooksService
{
    private readonly IMongoCollection<Book> _booksCollection;

    public BooksService(
        IOptions<BooksDatabaseSettings> booksDatabaseSettings)
    {
        var client = new MongoClient(
            booksDatabaseSettings.Value.ConnectionString);

        var database = client.GetDatabase(
            booksDatabaseSettings.Value.DatabaseName);

        _booksCollection = database.GetCollection<Book>(
            booksDatabaseSettings.Value.BooksCollectionName);

        // initialize the books database
        _booksCollection.DeleteMany(_ => true);

        List<Book> initBooklist = [];
        using (StreamReader sr = new StreamReader("files\\booklist.txt")!)
        {
            sr.ReadLine();
            while (!sr.EndOfStream)
            {
                string[] blocks = sr.ReadLine().Split("| ");
                Book book = new(blocks.Select(s => s.Trim()).ToArray());
                initBooklist.Add(book);
            }
        }
        
        _booksCollection.InsertMany(initBooklist);
    }

    public async Task<List<Book>> GetAsync() =>
        await _booksCollection.Find(_ => true).ToListAsync();

    public async Task<Book?> GetAsync(string barcode) =>
        await _booksCollection.Find(x => x.Acno == barcode).FirstOrDefaultAsync();

    public async Task CreateOneAsync(Book newBook) =>
        await _booksCollection.InsertOneAsync(newBook);

    public async Task CreateAsync(List<Book> newBooks) =>
        await _booksCollection.InsertManyAsync(newBooks);

    public async Task RemoveAllAsync() =>
        await _booksCollection.DeleteManyAsync(_ => true);
}