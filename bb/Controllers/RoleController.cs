using System.ComponentModel.DataAnnotations;
using bb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace bb.Controllers;

public class RoleController : Controller
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleController(RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public IActionResult CreateRole() => View();

    [HttpPost]
    public async Task<IActionResult> CreateRole([Required] string name)
    {
        if (ModelState.IsValid)
        {
            var result = await _roleManager.CreateAsync(new ApplicationRole() {Name = name});
            if (result.Succeeded)
                ViewBag.Message = "Role Created Successfully";
            else
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
        }

        return View();
    }
}