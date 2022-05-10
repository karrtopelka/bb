using bb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using bb.Services;
using Microsoft.AspNetCore.Identity;

namespace bb.Controllers;

[Authorize]
public class ProjectController : Controller
{
    private readonly ProjectService _projectService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProjectController(ProjectService projectService, UserManager<ApplicationUser> userManager)
    {
        _projectService = projectService;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return Redirect("Project/NewProject");
    }

    [HttpGet]
    public async Task<IActionResult> Project([FromQuery(Name = "projectId")] string projectId)
    {
        if (projectId is "" or null)
        {
            return Redirect("/");
        }

        var project = await _projectService.GetProject(projectId);
        return View(project);
    }

    public IActionResult NewProject()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> NewProject(string projectname, string projectdescription)
    {
        var author = await _userManager.GetUserAsync(HttpContext.User);
        var allUserProjects = await _projectService.GetUserProjectByName(author.Id, projectname);
        if (allUserProjects != null)
        {
            ModelState.AddModelError(nameof(projectname), "Project with this name already exists");
            return View();
        }

        var currentTime = DateTime.Now;
        var newProject = new Project
        {
            ProjectName = projectname,
            ProjectStatus = true,
            ProjectDescription = projectdescription,
            Author = author.Id,
            DateCreated = currentTime,
            Participants = new List<Guid>(),
            Logs = new List<string>()
        };
        var result = await _projectService.CreateProject(newProject);
        return Redirect($"/Project/Project?projectId={result.Id}");
    }

    public async Task<IActionResult> CloseProject(string id)
    {
        var newProject = await _projectService.CloseProject(id);
        return Redirect($"/Project/Project?projectId={newProject.Id}");
    }
    
    public async Task<IActionResult> ReopenProject(string id)
    {
        var newProject = await _projectService.ReopenProject(id);
        return Redirect($"/Project/Project?projectId={newProject.Id}");
    }
    
    public IActionResult AddLog(string id)
    {
        return Redirect($"/Log/AddLog?projectId={id}");
    }
    
    public IActionResult EditLog(string projectId, string logId)
    {
        return Redirect($"/Log/EditLog?projectId={projectId}&logId={logId}");
    }
    
    public async Task<IActionResult> RemoveLog(string projectId, string logId)
    {
        await _projectService.RemoveLog(projectId, logId);
        return Redirect($"/Project/Project?projectId={projectId}");
    }
}