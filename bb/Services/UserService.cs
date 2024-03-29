using bb.Models;
using MongoDB.Driver;

namespace bb.Services;

public interface IUserService
{
    Task<List<ApplicationUser>> GetAllUsers();
    Task<List<ApplicationUser>> GetAllUsersByEmails(List<string> emails);
    Task<ApplicationUser?> GetUser(Guid userId);
    Task<List<ApplicationUser>> GetUsersById(List<Guid>? users);
    Task UpdateUser(string email, ApplicationUser updatedUser);
    Task<List<ApplicationUser>> GetAllParticipants(List<Guid> participants);
    Task<ApplicationUser?> GetUserByUsername(string username);
}

public class UserService : IUserService
{
    private readonly IMongoCollection<ApplicationUser> _userCollection;

    public UserService(MyDatabaseSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var db = client.GetDatabase("acpe");
        _userCollection = db.GetCollection<ApplicationUser>("users");
    }

    public async Task<List<ApplicationUser>> GetAllUsers() =>
        await _userCollection.Find(_ => true).ToListAsync();

    public async Task<List<ApplicationUser>> GetAllUsersByEmails(List<string> emails) =>
        await _userCollection.Find(x => emails.Contains(x.Email)).ToListAsync();

    public async Task<ApplicationUser?> GetUser(Guid userId) =>
        await _userCollection.Find(x => x.Id == userId).FirstOrDefaultAsync();

    public async Task<List<ApplicationUser>> GetUsersById(List<Guid>? users) =>
        await _userCollection.Find(x => users.Contains(x.Id)).ToListAsync();

    public async Task UpdateUser(string email, ApplicationUser updatedUser) =>
        await _userCollection.ReplaceOneAsync(x => x.Email == email, updatedUser);

    public async Task<List<ApplicationUser>> GetAllParticipants(List<Guid> participants) =>
        await _userCollection.Find(x => participants.Contains(x.Id)).ToListAsync();

	public async Task<ApplicationUser?> GetUserByUsername(string username) =>
        await _userCollection.Find(x => x.UserName == username).FirstOrDefaultAsync();
}