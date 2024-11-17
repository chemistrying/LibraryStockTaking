using LibrarySystemApi.Models;
using MongoDB.Driver;

namespace LibrarySystemApi.Services;

public class AccountsService
{
    private readonly IMongoCollection<Account> _accountsCollection;

    public AccountsService(string collectionName)
    {

        var client = new MongoClient(
            LibraryDatabaseSettings.ConnectionString);

        var database = client.GetDatabase(
            LibraryDatabaseSettings.DatabaseName);

        _accountsCollection = database.GetCollection<Account>(
            collectionName);

        if (_accountsCollection.Find(x => x.IsAdmin).FirstOrDefault() == null)
        {
            Account newAdminAccount = new()
            {
                Name = "Admin",
                PasswordHash = PasswordHasher.HashPassword(Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "admin", PasswordHasher.GenerateSalt()), // you shouldn't use default password - change it through API endpoint or environmental variables
                IsAdmin = true,
                CreationTime = DateTime.Now
            };
            _accountsCollection.InsertOneAsync(newAdminAccount);
        }
    }

    public async Task<bool> LoginAsync(string name, string hash) =>
        await _accountsCollection.Find(x => x.Name == name && x.PasswordHash == hash).FirstOrDefaultAsync() != null;
    
    public async Task<Account> GetAsync(string name) =>
        await _accountsCollection.Find(x => x.Name == name).FirstOrDefaultAsync();

    public async Task CreateAsync(Account newAccount) =>
        await _accountsCollection.InsertOneAsync(newAccount);

    public async Task UpdateAsync(string id, Account updatedAccount) =>
        await _accountsCollection.ReplaceOneAsync(x => x.Id == id, updatedAccount);

    public async Task RemoveAsync(string name, string hash) =>
        await _accountsCollection.DeleteOneAsync(x => x.Name == name && x.PasswordHash == hash);
}