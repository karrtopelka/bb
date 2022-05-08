using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using bb.Models;
using bb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace bb.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ProjectService _projectService;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ILogger<HomeController> logger, ProjectService projectService,
        UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _projectService = projectService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var currentUserId = Guid.Parse(_userManager.GetUserId(HttpContext.User));
        var projects = await _projectService.GetAllUserProjects(currentUserId);
        projects.Reverse();
        ViewData["userId"] = currentUserId;
        return View(projects);
    }

    public IActionResult GoToProject(string id)
    {
        return Redirect($"/Project/Project?projectId={id}");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}