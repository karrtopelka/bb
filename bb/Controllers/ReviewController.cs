using System.Diagnostics;
using bb.Models;
using bb.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace bb.Controllers;

public class ReviewController : Controller
{
    private readonly ProjectService _projectService;

    public ReviewController(ProjectService projectService)
    {
        _projectService = projectService;
    }

    public async Task<IActionResult> Index([FromQuery(Name = "projectId")] string projectId)
    {
        if (projectId is "" or null)
        {
            return Redirect("/");
        }

        // get project info
        var project = await _projectService.GetProject(projectId);
        
        // add author as a participant
        project.Participants.Add(project.Author);

        // all members of the project
        var allMembers = project.Participants;
        
        var projectSumAmount = project.Logs.Select(x => x.Amount).Sum();
        ViewData["projectSumAmount"] = projectSumAmount;
        
        // average spent per user
        var average = projectSumAmount / allMembers.Count;
        
        // sum of money which user spent
        var expansesPerUser = new Dictionary<string, double>();
        foreach (var log in project.Logs)
        {
            if (expansesPerUser.ContainsKey(log.Who.UserName))
            {
                expansesPerUser[log.Who.UserName] += log.Amount;
            }
            else
            {
                expansesPerUser.Add(log.Who.UserName, log.Amount);
            }
        }

        // add zero expanses to users that did not pay for anything
        foreach (var member in allMembers.Where(member => !expansesPerUser.ContainsKey(member.UserName)))
        {
            expansesPerUser.Add(member.UserName, 0);
        }

        // sort descending expanses
        var sortedExpansesPerUser = from entry in expansesPerUser orderby entry.Value descending select entry;
        ViewData["expansesPerUser"] = sortedExpansesPerUser.ToDictionary(x => x.Key, x => x.Value);

        // how much money should return member
        var returnAmount = expansesPerUser.ToDictionary(expanse => expanse.Key, expanse => expanse.Value - average);

        // sum of all non negative sums
        var sumOfPositive = returnAmount.Where(a => a.Value > 0).Sum(a => a.Value);
        
        // check for negative coefficient for every user
        var coefficients = returnAmount.ToDictionary(a => a.Key, a => a.Value > 0 ? a.Value / sumOfPositive : 0);
        
        // evaluate return sum per user to all users
        var returnSums = new Dictionary<string, Dictionary<string, double>>();
        foreach (var ra in returnAmount)
        {
            var a = coefficients.Where(_ => ra.Value < 0).ToDictionary(c => c.Key, c => Math.Abs(ra.Value) * c.Value);
            returnSums.Add(ra.Key, a);
        }
        
        
        // filter zero sums
        var returnSumsFiltered = returnSums.Where(x => x.Value.Count > 0).ToDictionary(x => x.Key,
            x => x.Value.Where(xx => xx.Value > 0).ToDictionary(y => y.Key, y => y.Value));
        ViewData["Owes"] = returnSumsFiltered;

        return View(project);
    }

    public IActionResult BackToTheProject(string id) => Redirect($"/Project/Project?projectId={id}");
}