using bb.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace bb.Services;

public class ProjectService
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _db;
    private readonly IMongoCollection<Project> _projectCollection;
    private readonly UserService _userService;

    public ProjectService(MyDatabaseSettings settings, UserService userService)
    {
        _client = new MongoClient(settings.ConnectionString);
        _db = _client.GetDatabase("acpe");
        _projectCollection = _db.GetCollection<Project>("projects");
        _userService = userService;
    }

    public async Task<List<Project>> GetAllProjects() =>
        await _projectCollection.Find(_ => true).ToListAsync();

    public async Task<List<Project>> GetAllUserProjects(Guid userId) =>
        await _projectCollection.Find(x => x.Author == userId || x.Participants.Contains(userId)).ToListAsync();

    public async Task<Project?> GetUserProjectByName(Guid userId, string projectName) => await _projectCollection
        .Find(x => x.Author == userId && x.ProjectName == projectName).FirstOrDefaultAsync();

    public async Task<ProjectExtend?> GetProject(string? id)
    {
        var project = await _projectCollection.Find(x => x.Id == id)
            .FirstOrDefaultAsync();

        var projectMembers = await _userService.GetAllParticipants(project.Participants);
        var projectAuthor = await _userService.GetUser(project.Author);

        var projectExtended = new ProjectExtend
        {
            Id = project.Id,
            ProjectName = project.ProjectName,
            ProjectStatus = project.ProjectStatus,
            ProjectDescription = project.ProjectDescription,
            Author = projectAuthor,
            DateCreated = project.DateCreated,
            DateEnded = project.DateEnded,
            Participants = projectMembers,
            Logs = project.Logs
        };

        return projectExtended;
    }

    public async Task<Project> CreateProject(Project newProject)
    {
        await _projectCollection.InsertOneAsync(newProject);
        return newProject;
    }

    public async Task<Project> CloseProject(string id)
    {
        var filter = Builders<Project>.Filter.Where(_ => _.Id == id);
        var update =  Builders<Project>.Update.Set(_ => _.ProjectStatus, false);
        var options = new FindOneAndUpdateOptions<Project>
        {
            ReturnDocument = ReturnDocument.After
        };

        var updatedProject = await _projectCollection.FindOneAndUpdateAsync(filter, update, options);
        return updatedProject;
    }
    
    public async Task<Project> ReopenProject(string id)
    {
        var filter = Builders<Project>.Filter.Where(_ => _.Id == id);
        var update =  Builders<Project>.Update.Set(_ => _.ProjectStatus, true);
        var options = new FindOneAndUpdateOptions<Project>
        {
            ReturnDocument = ReturnDocument.After
        };

        var updatedProject = await _projectCollection.FindOneAndUpdateAsync(filter, update, options);
        return updatedProject;
    }
    
    public async Task UpdateProject(string id, Project updatedProject) =>
        await _projectCollection.ReplaceOneAsync(x => x.Id == id, updatedProject);

    public async Task RemoveProject(string id) =>
        await _projectCollection.DeleteOneAsync(x => x.Id == id);
}