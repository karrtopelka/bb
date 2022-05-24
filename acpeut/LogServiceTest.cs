using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bb.Models;
using bb.Services;
using Moq;
using Xunit;

namespace acpeut;

public class LogServiceTest
{
    private const string TestProjectId = "627d47be420a155f23c9f02b";
    private const string TestLogId = "627d47ae420a155f23c9f02b";
    private static readonly Guid TestLogWho = new("e9da77e6-5cac-4af0-ae8a-30d322d13625");
    private static readonly ApplicationUser AppUser = new() {UserName = "karrtopelka", Email = "karrtopelka@gmail.com"};
    
    private readonly Log _testLog = new()
    {
        Id = TestLogId,
        LogDate = DateTime.Now,
        Who = TestLogWho,
        Amount = 200.22,
        Purpose = "restaurant"
    };
    
    private readonly LogExtend _testLogExtended = new()
    {
        Id = TestLogId,
        LogDate = DateTime.Now,
        Who = AppUser,
        Amount = 200.22,
        Purpose = "restaurant"
    };
    
    [Fact]
    public async void GetLog_Test()
    {
        var mock = new Mock<ILogService>();
        mock.Setup(x => x.GetLog(TestLogId)).ReturnsAsync(_testLog);
        var projectService = mock.Object;
        var mockLog = await projectService.GetLog(TestLogId);

        Assert.Equal(TestLogId, mockLog.Id);
    }
    
    [Fact]
    public async void AddLog_Test()
    {
        var mock = new Mock<ILogService>();
        mock.Setup(x => x.AddLog(_testLog, TestProjectId));
        var projectService = mock.Object;
        await Assert.IsAssignableFrom<Task>(projectService.AddLog(_testLog, TestProjectId));
    }
    
    [Fact]
    public async void EditLog_Test()
    {
        var mock = new Mock<ILogService>();
        mock.Setup(x => x.EditLog(_testLog));
        var projectService = mock.Object;
        await Assert.IsAssignableFrom<Task>(projectService.EditLog(_testLog));
    }
    
    [Fact]
    public async void GetProjectLogs_Test()
    {
        var logIds = new List<string>{_testLog.Id, _testLog.Id};
        var mock = new Mock<ILogService>();
        mock.Setup(x => x.GetProjectLogs(logIds)).ReturnsAsync(new List<LogExtend>{_testLogExtended, _testLogExtended});
        var projectService = mock.Object;
        var mockLog = await projectService.GetProjectLogs(logIds);

        if (mockLog != null) Assert.Equal(logIds.Count, mockLog.Count);
    }
    
    [Fact]
    public async void RemoveLog_Test()
    {
        var mock = new Mock<ILogService>();
        mock.Setup(x => x.RemoveLog(TestLogId));
        var projectService = mock.Object;
        await Assert.IsAssignableFrom<Task>(projectService.RemoveLog(TestLogId));
    }
}