using System.Diagnostics;
using bb.Models;
using bb.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace bb.Controllers;

public class ReviewController : Controller
{
    private readonly ProjectService _projectService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly UserService _userService;
    private readonly LogService _logService;

    public ReviewController(ProjectService projectService, UserManager<ApplicationUser> userManager,
        UserService userService, LogService logService)
    {
        _projectService = projectService;
        _userManager = userManager;
        _userService = userService;
        _logService = logService;
    }

    public async Task<IActionResult> Index([FromQuery(Name = "projectId")] string projectId)
    {
        if (projectId is "" or null)
        {
            return Redirect("/");
        }

        var project = await _projectService.GetProject(projectId);
        project.Participants.Add(project.Author);

        var logsGuid = project.Logs.Select(x => x.Who.Id).ToList();
        var allMembers = await _userService.GetAllParticipants(logsGuid);

        var projectSumAmount = project.Logs.Select(x => x.Amount).Sum();
        ViewData["projectSumAmount"] = projectSumAmount;

        var average = projectSumAmount / allMembers.Count;

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

        foreach (var member in allMembers.Where(member => !expansesPerUser.ContainsKey(member.UserName)))
        {
            expansesPerUser.Add(member.UserName, 0);
        }

        var sortedExpansesPerUser = from entry in expansesPerUser orderby entry.Value descending select entry;
        ViewData["expansesPerUser"] = sortedExpansesPerUser.ToDictionary(x => x.Key, x => x.Value);

        var returnAmount = expansesPerUser.ToDictionary(expanse => expanse.Key, expanse => expanse.Value - average);

        var sumOfPositive = returnAmount.Where(a => a.Value > 0).Sum(a => a.Value);

        var coefs = returnAmount.ToDictionary(a => a.Key, a => a.Value > 0 ? a.Value / sumOfPositive : 0);

        var returnSums = new Dictionary<string, Dictionary<string, double>>();
        foreach (var ra in returnAmount)
        {
            var a = coefs.Where(c => ra.Value < 0).ToDictionary(c => c.Key, c => Math.Abs(ra.Value) * c.Value);
            returnSums.Add(ra.Key, a);
        }

        var returnSumsFiltered = returnSums.Where(x => x.Value.Count > 0).ToDictionary(x => x.Key,
            x => x.Value.Where(xx => xx.Value > 0).ToDictionary(y => y.Key, y => y.Value));

        ViewData["Owes"] = returnSumsFiltered;

        return View(project);
    }

    public IActionResult BackToTheProject(string id) => Redirect($"/Project/Project?projectId={id}");
}