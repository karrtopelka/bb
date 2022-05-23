using System;
using System.Collections.Generic;
using bb.Models;
using MongoDB.Bson;
using Xunit;

namespace acpeut;

public class ModelsTest
{
    [Fact]
    public void UserTest()
    {
        var nguid = new Guid();
        var testUser = new User
        {
            _id = nguid,
            Email = "a@a.com",
            Password = "password",
            UserName = "a"
        };
        
        Assert.Equal(nguid, testUser._id);
        Assert.Equal("a@a.com", testUser.Email);
        Assert.Equal("password", testUser.Password);
        Assert.Equal("a", testUser.UserName);
    }

    [Fact]
    public void ProjectTest()
    {
        var nguid = new Guid();
        var participantsList = new List<Guid>();
        var dateTimeCreateProject = DateTime.Now;

        for (var i = 0; i < 3; i++)
        {
            participantsList.Add(new Guid());
        }
        
        var nid = ObjectId.GenerateNewId();
        var testProject = new Project
        {
            Id = nid.ToString(),
            ProjectName = "Test Project",
            ProjectStatus = true,
            ProjectDescription = "Test Project Description",
            Author = nguid,
            DateCreated = dateTimeCreateProject,
            DateEnded = null,
            Participants = participantsList,
            Logs = new List<string>()
        };
        
        Assert.Equal(nid.ToString(), testProject.Id);
        Assert.Equal("Test Project", testProject.ProjectName);
        Assert.True(testProject.ProjectStatus);
        Assert.Equal("Test Project Description", testProject.ProjectDescription);
        Assert.Equal(nguid, testProject.Author);
        Assert.Equal(dateTimeCreateProject, testProject.DateCreated);
        Assert.Null(testProject.DateEnded);
        Assert.NotEmpty(testProject.Participants);
        Assert.Empty(testProject.Logs);
    }

    [Fact]
    public void LogTest()
    {
        var nid = ObjectId.GenerateNewId();
        var nguid = new Guid();
        var dateTimeLogCreate = DateTime.Now;
        var testLog = new Log
        {
            Id = nid.ToString(),
            LogDate = dateTimeLogCreate,
            Who = nguid,
            Amount = 222.22,
            Purpose = "restaurant"
        };
        
        Assert.Equal(nguid, testLog.Who);
        Assert.Equal("restaurant", testLog.Purpose);
        Assert.Equal(dateTimeLogCreate, testLog.LogDate);
        Assert.Equal(nid.ToString(), testLog.Id);
        Assert.Equal(222.22, testLog.Amount);
    }
}