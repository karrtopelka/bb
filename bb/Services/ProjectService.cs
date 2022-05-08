using bb.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace bb.Services;

public class ProjectService
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _db;
    private readonly IMongoCollection<Project> _projectCollection;

    public ProjectService(MyDatabaseSettings settings)
    {
        _client = new MongoClient(settings.ConnectionString);
        _db = _client.GetDatabase("acpe");
        _projectCollection = _db.GetCollection<Project>("projects");
    }

    public async Task<List<Project>> GetAllProjects() =>
        await _projectCollection.Find(_ => true).ToListAsync();

    public async Task<List<Project>> GetAllUserProjects(Guid userId) =>
        await _projectCollection.Find(x => x.Author == userId || x.Participants.Contains(userId)).ToListAsync();

    public async Task<Project?> GetUserProjectByName(Guid userId, string projectName) =>
        await _projectCollection.Find(x => x.Author == userId && x.ProjectName == projectName).FirstOrDefaultAsync();

    public async Task<Project?> GetProject(string? id) =>
        await _projectCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<Project> CreateProject(Project newProject)
    {
        await _projectCollection.InsertOneAsync(newProject);
        return newProject;
    }


    public async Task UpdateProject(string id, Project updatedProject) =>
        await _projectCollection.ReplaceOneAsync(x => x.Id == id, updatedProject);

    public async Task RemoveProject(string id) =>
        await _projectCollection.DeleteOneAsync(x => x.Id == id);
}