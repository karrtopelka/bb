using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bb.Models;
using bb.Services;
using Moq;
using Xunit;

namespace acpeut;

public class UserServiceTest
{
    private readonly ApplicationUser _appUser = new() {UserName = "karrtopelka", Email = "karrtopelka@gmail.com"};
    
    [Fact]
    public async void GetAllUsers_Test()
    {
        var mock = new Mock<IUserService>();
        mock.Setup(x => x.GetAllUsers()).ReturnsAsync(new List<ApplicationUser>{_appUser});
        var projectService = mock.Object;
        var mockUser = await projectService.GetAllUsers();

        Assert.Single(mockUser);
    }
    
    [Fact]
    public async void GetAllUsersByEmails_Test()
    {
        var emails = new List<string>
        {
            "a@a.com", "b@b.com"
        };
        var mock = new Mock<IUserService>();
        mock.Setup(x => x.GetAllUsersByEmails(emails)).ReturnsAsync(new List<ApplicationUser>{_appUser, _appUser});
        var projectService = mock.Object;
        var mockUser = await projectService.GetAllUsersByEmails(emails);

        Assert.Equal(2, mockUser.Count);
    }
    
    [Fact]
    public async void GetUser_Test()
    {
        var mock = new Mock<IUserService>();
        mock.Setup(x => x.GetUser(_appUser.Id)).ReturnsAsync(_appUser);
        var projectService = mock.Object;
        var mockUser = await projectService.GetUser(_appUser.Id);

        Assert.Equal(_appUser.UserName, mockUser?.UserName);
    }
    
    [Fact]
    public async void GetUsersById_Test()
    {
        var userIds = new List<Guid> {_appUser.Id, _appUser.Id};
        var mock = new Mock<IUserService>();
        mock.Setup(x => x.GetUsersById(userIds)).ReturnsAsync(new List<ApplicationUser>{_appUser, _appUser});
        var projectService = mock.Object;
        var mockUser = await projectService.GetUsersById(userIds);

        Assert.Equal(userIds.Count, mockUser.Count);
    }
    
    [Fact]
    public async void UpdateUser_Test()
    {
        var mock = new Mock<IUserService>();
        mock.Setup(x => x.UpdateUser(_appUser.Email, _appUser));
        var projectService = mock.Object;

        await Assert.IsAssignableFrom<Task>(projectService.UpdateUser(_appUser.Email, _appUser));
    }
    
    [Fact]
    public async void GetAllParticipants_Test()
    {
        var userIds = new List<Guid> {_appUser.Id, _appUser.Id};
        var mock = new Mock<IUserService>();
        mock.Setup(x => x.GetAllParticipants(userIds)).ReturnsAsync(new List<ApplicationUser>{_appUser, _appUser});;
        var projectService = mock.Object;

        var mockUsers = await projectService.GetAllParticipants(userIds);
        
        Assert.Equal(userIds.Count, mockUsers.Count);
    }
    
    [Fact]
    public async void GetUserByUsername_Test()
    {
        var mock = new Mock<IUserService>();
        mock.Setup(x => x.GetUserByUsername("karrtopelka")).ReturnsAsync(_appUser);;
        var projectService = mock.Object;

        var mockUsers = await projectService.GetUserByUsername("karrtopelka");
        
        Assert.Equal("karrtopelka", _appUser.UserName);
    }
}