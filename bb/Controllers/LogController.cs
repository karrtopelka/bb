using bb.Models;
using bb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bb.Controllers;

[Authorize]
public class LogController : Controller
{
    private readonly LogService _logService;
    private readonly ProjectService _projectService;

    public LogController(LogService logService, ProjectService projectService)
    {
        _logService = logService;
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<IActionResult> AddLog([FromQuery(Name = "projectId")] string projectId)
    {
        var participants = await _projectService.GetProjectParticipants(projectId);
        ViewData["participants"] = participants;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddLog(Log log, [FromQuery(Name = "projectId")] string projectId)
    {
        log.LogDate = DateTime.Now;
        await _logService.AddLog(log, projectId);

        return Redirect($"/Project/Project?projectId={projectId}");
    }

    [HttpGet]
    public async Task<IActionResult> EditLog([FromQuery(Name = "projectId")] string projectId,
        [FromQuery(Name = "logId")] string logId)
    {
        var logData = await _logService.GetLog(logId);
        var participants = await _projectService.GetProjectParticipants(projectId);
        ViewData["participants"] = participants;
        return View(logData);
    }

    [HttpPost]
    public async Task<IActionResult> EditLog(Log log, [FromQuery(Name = "projectId")] string projectId)
    {
        await _logService.EditLog(log);

        return Redirect($"/Project/Project?projectId={projectId}");
    }
}