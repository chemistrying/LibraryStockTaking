using LibrarySystemApi.Models;
using MongoDB.Driver;

namespace LibrarySystemApi.Services;

public class BookshelvesService
{
    private readonly IMongoCollection<Bookshelf> _bookshelvesCollection;

    public BookshelvesService(string collectionName)
    {
        var client = new MongoClient(
            LibraryDatabaseSettings.ConnectionString);

        var database = client.GetDatabase(
            LibraryDatabaseSettings.DatabaseName);

        _bookshelvesCollection = database.GetCollection<Bookshelf>(
            collectionName);
    }

    public async Task<List<Bookshelf>> GetAsync() =>
        await _bookshelvesCollection.Find(_ => true).ToListAsync();

    public async Task<Bookshelf?> GetAsync(string id) =>
        await _bookshelvesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<List<Bookshelf>> GetSessionBookshelvesAsync(string sessionId) =>
        await _bookshelvesCollection.Find(x => x.SessionId == sessionId).ToListAsync();

    public async Task<List<Bookshelf>> GetGroupBookshelvesAsync(string sessionId, string groupName) =>
        await _bookshelvesCollection.Find(x => x.SessionId == sessionId && x.GroupName == groupName).ToListAsync();

    public async Task CreateAsync(Bookshelf newBookshelf) =>
        await _bookshelvesCollection.InsertOneAsync(newBookshelf);

    public async Task CreateManyAsync(List<Bookshelf> newBookshelves) =>
        await _bookshelvesCollection.InsertManyAsync(newBookshelves);

    public async Task UpdateAsync(string id, Bookshelf updatedBookshelf) =>
        await _bookshelvesCollection.ReplaceOneAsync(x => x.Id == id, updatedBookshelf);

    public async Task RemoveAsync(string id) =>
        await _bookshelvesCollection.DeleteOneAsync(x => x.Id == id);
    
    public async Task RemoveManyAsync(string groupName) =>
        await _bookshelvesCollection.DeleteManyAsync(x => x.GroupName == groupName);
}
