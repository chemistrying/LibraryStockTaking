using LibrarySystemApi.Models;
using MongoDB.Driver;

namespace LibrarySystemApi.Services;

public class TokensService
{
    private readonly IMongoCollection<Token> _tokensCollection;

    public TokensService(string collectionName)
    {

        var client = new MongoClient(
            LibraryDatabaseSettings.ConnectionString);

        var database = client.GetDatabase(
            LibraryDatabaseSettings.DatabaseName);

        _tokensCollection = database.GetCollection<Token>(
            collectionName);
    }

    public async Task<bool> ValidAsync(string id) =>
        await _tokensCollection.Find(x => x.Id == id && x.ExpiryDate <= DateTime.Now).FirstOrDefaultAsync() != null;

    public async Task CreateAsync(Token token) =>
        await _tokensCollection.InsertOneAsync(token);

    public async Task RemoveAsync(string id) =>
        await _tokensCollection.DeleteOneAsync(x => x.Id == id);

    public async Task RemoveAllAsync(string owner) =>
        await _tokensCollection.DeleteManyAsync(x => x.Owner == owner);
}