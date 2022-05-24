using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bb.Models;
using bb.Services;
using Moq;
using Xunit;

namespace acpeut;

public class ProjectServiceTest
{
    private const string TestProjectId = "627d47ae420a155f23c9f02b";
    private const string TestLogId = "627d47be420a155f23c9f02b";
    private const string TestProjectName = "Rakivec";
    private readonly Guid _testProjectAuthor = new("e9da77e6-5cac-4af0-ae8a-30d322d13625");
    private readonly ApplicationUser _appUser = new() {UserName = "karrtopelka", Email = "karrtopelka@gmail.com"};
    
    private readonly Project _testProject = new()
    {
        Id = TestProjectId,
        ProjectName = "Rakivec",
        ProjectStatus = true,
        ProjectDescription = "Zhovkva",
        Author = new Guid("e9da77e6-5cac-4af0-ae8a-30d322d13625"),
        DateCreated = DateTime.Parse("2022-05-12T17:45:18.746+00:00"),
        DateEnded = null,
        Participants = new List<Guid>{new()},
        Logs = new List<string>()
    };

    private readonly Project _testProjectClosed = new()
    {
        Id = TestProjectId,
        ProjectName = "Rakivec",
        ProjectStatus = false,
        ProjectDescription = "Zhovkva",
        Author = new Guid("e9da77e6-5cac-4af0-ae8a-30d322d13625"),
        DateCreated = DateTime.Parse("2022-05-12T17:45:18.746+00:00"),
        DateEnded = DateTime.Parse("2022-05-18T17:45:18.746+00:00"),
        Participants = new List<Guid>{new()},
        Logs = new List<string>()
    };

    private readonly ProjectExtend _testProjectExtended = new()
    {
        Id = TestProjectId,
        ProjectName = "Rakivec",
        ProjectStatus = true,
        ProjectDescription = "Zhovkva",
        Author = new ApplicationUser { UserName = "karrtopelka"},
        DateCreated = DateTime.Parse("2022-05-12T17:45:18.746+00:00"),
        DateEnded = null,
        Participants = new List<ApplicationUser>{new()},
        Logs = new List<LogExtend>()
    };

    [Fact]
    public async void GetRawProject_Test()
    {
        var mock = new Mock<IProjectService>();
        mock.Setup(x => x.GetRawProject(TestProjectId)).ReturnsAsync(_testProject);
        var projectService = mock.Object;
        var mockProject = await projectService.GetRawProject(TestProjectId);

        Assert.Equal(_testProject.Id, mockProject.Id);
        Assert.Equal("Rakivec", mockProject.ProjectName);
        Assert.True(mockProject.ProjectStatus);
        Assert.Equal("Zhovkva", mockProject.ProjectDescription);
        Assert.Null(mockProject.DateEnded);
    }
    
    [Fact]
    public async void GetAllUserProjects_Test()
    {
        var mock = new Mock<IProjectService>();
        mock.Setup(x => x.GetAllUserProjects(_testProjectAuthor)).ReturnsAsync(new List<Project>{_testProject});
        var projectService = mock.Object;
        var mockProjects = await projectService.GetAllUserProjects(_testProjectAuthor);

        Assert.NotEmpty(mockProjects);
    }
    
    [Fact]
    public async void GetUserProjectByName_Test()
    {
        var mock = new Mock<IProjectService>();
        mock.Setup(x => x.GetUserProjectByName(_testProjectAuthor, TestProjectName)).ReturnsAsync(_testProject);
        var projectService = mock.Object;
        var mockProject = await projectService.GetUserProjectByName(_testProjectAuthor, TestProjectName);

        Assert.Equal(_testProject.Id, mockProject.Id);
    }
    
    [Fact]
    public async void GetProject_Test()
    {
        var mock = new Mock<IProjectService>();
        mock.Setup(x => x.GetProject(TestProjectId)).ReturnsAsync(_testProjectExtended);
        var projectService = mock.Object;
        var mockProject = await projectService.GetProject(TestProjectId);

        Assert.Equal(_testProjectExtended.Id, mockProject.Id);
    }
    
    [Fact]
    public async void CreateProject_Test()
    {
        var currentTime = DateTime.Now;
        var newProject = new Project
        {
            ProjectName = "name",
            ProjectStatus = true,
            ProjectDescription = "projectDescription",
            Author = _testProjectAuthor,
            DateCreated = currentTime,
            Participants = new List<Guid>(),
            Logs = new List<string>()
        };
        
        var mock = new Mock<IProjectService>();
        mock.Setup(x => x.CreateProject(newProject)).ReturnsAsync(_testProject);
        var projectService = mock.Object;
        var mockProject = await projectService.CreateProject(newProject);

        Assert.Equal(_testProject.Id, mockProject.Id);
    }
    
    [Fact]
    public async void CloseProject_Test()
    {
        var mock = new Mock<IProjectService>();
        mock.Setup(x => x.CloseProject(TestProjectId)).ReturnsAsync(_testProjectClosed);
        var projectService = mock.Object;
        var mockProject = await projectService.CloseProject(TestProjectId);

        Assert.False(mockProject.ProjectStatus);
        Assert.NotNull(mockProject.DateEnded);
    }
    
    [Fact]
    public async void ReopenProject_Test()
    {
        var mock = new Mock<IProjectService>();
        mock.Setup(x => x.ReopenProject(TestProjectId)).ReturnsAsync(_testProject);
        var projectService = mock.Object;
        var mockProject = await projectService.ReopenProject(TestProjectId);

        Assert.True(mockProject.ProjectStatus);
        Assert.Null(mockProject.DateEnded);
    }
    
    [Fact]
    public async void GetProjectParticipants_Test()
    {
        var mock = new Mock<IProjectService>();
        mock.Setup(x => x.GetProjectParticipants(TestProjectId)).ReturnsAsync(new List<ApplicationUser>{_appUser});
        var projectService = mock.Object;
        var mockProject = await projectService.GetProjectParticipants(TestProjectId);

        Assert.Single(mockProject);
    }
    
    [Fact]
    public async void RemoveLog_Test()
    {
        var mock = new Mock<IProjectService>();
        mock.Setup(x => x.RemoveLog(TestProjectId, TestLogId));
        var projectService = mock.Object;

        await Assert.IsAssignableFrom<Task>(projectService.RemoveLog(TestProjectId, TestLogId));
    }
    
    [Fact]
    public async void UpdateProject_Test()
    {
        var mock = new Mock<IProjectService>();
        mock.Setup(x => x.UpdateProject(TestProjectId, _testProject));
        var projectService = mock.Object;

        await Assert.IsAssignableFrom<Task>(projectService.UpdateProject(TestProjectId, _testProject));
    }
}