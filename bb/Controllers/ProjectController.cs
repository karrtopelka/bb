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
    private readonly UserService _userService;

    public ProjectController(ProjectService projectService, UserManager<ApplicationUser> userManager, UserService userService)
    {
        _projectService = projectService;
        _userManager = userManager;
        _userService = userService;
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
    
	[HttpPost]
    public async Task<IActionResult> NewParticipant(string projectId, string username)
    {
        var user = await _userService.GetUserByUsername(username);
        if (user == null)
        {
            return Redirect($"Project/Project?projectId={projectId}");
        }
		var project = await _projectService.GetRawProject(projectId);
        project.Participants.Add(user.Id);
        await _projectService.UpdateProject(projectId, project);
        return Redirect($"Project/Project?projectId={projectId}");
    }
    [HttpGet]
    public async Task<IActionResult> RemoveParticipant(string projectId, Guid userId)
    {
        var project = await _projectService.GetRawProject(projectId);
        project.Participants = project.Participants.Where(id => id != userId).ToList();
        await _projectService.UpdateProject(projectId, project);
        return Redirect($"Project/Project?projectId={projectId}");
    }
}
