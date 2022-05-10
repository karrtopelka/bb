using System.ComponentModel.DataAnnotations;
using bb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace bb.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    public ViewResult RegisterUser() => View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUser(User user)
    {
        if (ModelState.IsValid)
        {
            var appUser = new ApplicationUser
            {
                UserName = user.UserName,
                Email = user.Email
            };

            var result = await _userManager.CreateAsync(appUser, user.Password);
            if (result.Succeeded)
            {
                ViewBag.Message = "User Created Successfully";
                var resultSignIn =
                    await _signInManager.PasswordSignInAsync(appUser, user.Password, false, false);
                return Redirect(resultSignIn.Succeeded ? "/" : "/Login");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        return View(user);
    }


    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([Required] [EmailAddress] string email, [Required] string password,
        string? returnurl)
    {
        if (ModelState.IsValid)
        {
            var appUser = await _userManager.FindByEmailAsync(email);
            if (appUser != null)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(appUser, password, false, false);
                if (result.Succeeded)
                {
                    return Redirect(returnurl ?? "/");
                }
            }

            ModelState.AddModelError(nameof(email), "Login Failed: Invalid Email or Password");
        }

        return View();
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}