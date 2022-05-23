using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bb.Models;
using bb.Services;
using MongoDB.Bson;
using Moq;
using Xunit;

namespace acpeut;

public class ServicesTest
{
    const string testProjectId = "627d47ae420a155f23c9f02b";

    private Project _testProject = new()
    {
        Id = testProjectId,
        ProjectName = "Rakivec",
        ProjectStatus = true,
        ProjectDescription = "Zhovkva",
        Author = new Guid("e9da77e6-5cac-4af0-ae8a-30d322d13625"),
        DateCreated = DateTime.Parse("2022-05-12T17:45:18.746+00:00"),
        DateEnded = null,
        Participants = new List<Guid>{new()},
        Logs = new List<string>()
    };

    [Fact]
    public async void UserServiceTest()
    {
        const string testProjectId = "627d47ae420a155f23c9f02b";

        var mock = new Mock<ProjectService>();
        mock.Setup(x => x.GetRawProject(testProjectId)).ReturnsAsync(_testProject);
        var projectService = mock.Object;
        var mockProject = await projectService.GetRawProject(testProjectId);
        
        // var testProjectObjectId = new ObjectId(testProjectId);
        // var authorId = new Guid("e9da77e6-5cac-4af0-ae8a-30d322d13625");
        // var projectDate = DateTime.Parse("2022-05-12T17:45:18.746+00:00");
        // var getRawProject = await _projectService.GetRawProject(testProjectId);
        
        Assert.Equal(_testProject.Id, mockProject.Id);
        // Assert.Equal("Rakivec", mockProject.ProjectName);
        // Assert.True(mockProject.ProjectStatus);
        // Assert.Equal("Zhovkva", mockProject.ProjectDescription);
        // Assert.Equal(authorId, mockProject.Author);
        // Assert.Equal(projectDate, mockProject.DateCreated);
        // Assert.Null(mockProject.DateEnded);
        // if (mockProject.Participants != null) Assert.NotEmpty(mockProject.Participants);
        // if (mockProject.Logs != null) Assert.NotEmpty(mockProject.Logs);
    }
}