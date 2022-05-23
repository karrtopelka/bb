using bb.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace bb.Services;

public interface IProjectService
{
    Task<Project> GetRawProject(string projectId);
    Task<List<Project>> GetAllUserProjects(Guid userId);
    Task<Project> GetUserProjectByName(Guid userId, string projectName);
    Task<ProjectExtend> GetProject(string? id);
    Task<Project> CreateProject(Project newProject);
    Task<Project> CloseProject(string id);
    Task<Project> ReopenProject(string id);
    Task<List<ApplicationUser>> GetProjectParticipants(string id);
    Task RemoveLog(string id, string logId);
    Task UpdateProject(string id, Project updatedProject);
}

public class ProjectService : IProjectService
{
    private readonly IMongoCollection<Project> _projectCollection;
    private readonly UserService _userService;
    private readonly LogService _logService;

    public ProjectService(MyDatabaseSettings settings, UserService userService, LogService logService)
    {
        var client = new MongoClient(settings.ConnectionString);
        var db = client.GetDatabase("acpe");
        _projectCollection = db.GetCollection<Project>("projects");
        _userService = userService;
        _logService = logService;
    }

    public async Task<Project> GetRawProject(string projectId) =>
        await _projectCollection.Find(x => x.Id == projectId).FirstOrDefaultAsync();

    public async Task<List<Project>> GetAllUserProjects(Guid userId) =>
        await _projectCollection.Find(x => x.Author == userId || x.Participants.Contains(userId)).ToListAsync();

    public async Task<Project> GetUserProjectByName(Guid userId, string projectName) => await _projectCollection
        .Find(x => x.Author == userId && x.ProjectName == projectName).FirstOrDefaultAsync();

    public async Task<ProjectExtend> GetProject(string? id)
    {
        var project = await _projectCollection.Find(x => x.Id == id)
            .FirstOrDefaultAsync();

        var projectMembers = await _userService.GetAllParticipants(project.Participants);
        var projectAuthor = await _userService.GetUser(project.Author);

        var projectLogs = await _logService.GetProjectLogs(project.Logs);

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
            Logs = projectLogs
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
        var update = Builders<Project>.Update.Set(_ => _.ProjectStatus, false).Set(_ => _.DateEnded, DateTime.Now);
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
        var update = Builders<Project>.Update.Set(_ => _.ProjectStatus, true).Set(_ => _.DateEnded, null);
        var options = new FindOneAndUpdateOptions<Project>
        {
            ReturnDocument = ReturnDocument.After
        };

        var updatedProject = await _projectCollection.FindOneAndUpdateAsync(filter, update, options);
        return updatedProject;
    }

    public async Task<List<ApplicationUser>> GetProjectParticipants(string id)
    {
        var project = await _projectCollection.Find(x => x.Id == id)
            .FirstOrDefaultAsync();

        project.Participants.Add(project.Author);

        var participants = await _userService.GetUsersById(project.Participants);

        return participants;
    }
    
    public async Task RemoveLog(string id, string logId)
    {
        var project = await _projectCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        var filteredLogs = project.Logs.Where(x => x != logId).ToList();
        
        var filter = Builders<Project>.Filter.Where(_ => _.Id == id);
        var update = Builders<Project>.Update.Set(_ => _.Logs, filteredLogs);
        var options = new FindOneAndUpdateOptions<Project>();

        await _projectCollection.FindOneAndUpdateAsync(filter, update, options);
    }

    public async Task UpdateProject(string id, Project updatedProject) =>
        await _projectCollection.ReplaceOneAsync(x => x.Id == id, updatedProject);

    public async Task RemoveProject(string id) =>
        await _projectCollection.DeleteOneAsync(x => x.Id == id);
}