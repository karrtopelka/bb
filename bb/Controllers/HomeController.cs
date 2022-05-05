using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using bb.Models;
using bb.Services;

namespace bb.Controllers;

public class HomeController : Controller
{
    private readonly DocumentService _documentService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View(new User());
    }
    
    [HttpPost]
    public async Task<IActionResult> Index(
        User user
    )
    {
        await _documentService.CreateUser(user);
        var count = await _documentService.GetCollectionCount("users");
        return RedirectToAction("Index", GetRouteValues("acpe", "users", count - 1));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    private static object GetRouteValues(string database, string collection, long index)
    {
        return new { selectedDatabase = database, selectedCollection = collection, index = index };
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}