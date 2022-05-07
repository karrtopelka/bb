using bb.Models;
using MongoDB.Driver;

namespace bb.Services;

public class UserService
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _db;
    private readonly IMongoCollection<User> _userCollection;
    
    public UserService(MyDatabaseSettings settings)
    {
        _client = new MongoClient(settings.ConnectionString);
        _db = _client.GetDatabase("acpe");
        _userCollection = _db.GetCollection<User>("users");
    }
    
    public async Task<List<User>> GetAllUsers() =>
        await _userCollection.Find(_ => true).ToListAsync();
    
    public async Task<List<User>> GetAllUsersByEmails(List<string> emails) =>
        await _userCollection.Find(x => emails.Contains(x.Email)).ToListAsync();

    public async Task<User?> GetUser(string username) =>
        await _userCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

    public async Task UpdateUser(string email, User updatedUser) =>
        await _userCollection.ReplaceOneAsync(x => x.Email == email, updatedUser);
}