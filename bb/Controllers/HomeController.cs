using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using bb.Models;
using bb.Services;
using Microsoft.AspNetCore.Authorization;

namespace bb.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly DocumentService _documentService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, DocumentService documentService)
    {
        _logger = logger;
        _documentService = documentService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}