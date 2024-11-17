using LibrarySystemApi.Models;
using MongoDB.Driver;

namespace LibrarySystemApi.Services;

public class SessionsService
{
    private readonly IMongoCollection<StockTakingSession> _sessionsCollection;

    public SessionsService(string collectionName)
    {

        var client = new MongoClient(
            LibraryDatabaseSettings.ConnectionString);
        // Serilog.Log.Information("HI" + sessionsDatabaseSettings.Value.SessionsCollectionName);

        var database = client.GetDatabase(
            LibraryDatabaseSettings.DatabaseName);

        _sessionsCollection = database.GetCollection<StockTakingSession>(
            collectionName);
    }

    public async Task<List<StockTakingSession>> GetAsync() =>
        await _sessionsCollection.Find(_ => true).ToListAsync();

    public async Task<StockTakingSession?> GetAsync(string id) =>
        await _sessionsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<StockTakingSession?> GetActiveAsync() =>
        await _sessionsCollection.Find(x => x.IsActive).FirstOrDefaultAsync();
    
    public async Task CreateAsync(StockTakingSession newSession) =>
        await _sessionsCollection.InsertOneAsync(newSession);

    public async Task UpdateAsync(string id, StockTakingSession updatedSession) =>
        await _sessionsCollection.ReplaceOneAsync(x => x.Id == id, updatedSession);

    public async Task RemoveAsync(string id) =>
        await _sessionsCollection.DeleteOneAsync(x => x.Id == id);
}