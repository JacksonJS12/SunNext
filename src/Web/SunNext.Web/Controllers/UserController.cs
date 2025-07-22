using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Manage.Internal;
using Microsoft.AspNetCore.Mvc;
using SunNext.Common;

namespace SunNext.Web.Controllers;

public class UserController : BaseController
{
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public IActionResult AdminOverview()
    {
        return View("Admin/Overview");
    }

    public IActionResult AdminUsers()
    {
        return View("Admin/Users");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(string username, string password)
    {
        // TODO: Replace with real authentication
        if (username == "admin" && password == "password")
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(string username, string password, string confirmPassword)
    {
        // TODO: Replace with real user registration logic
        if (password != confirmPassword)
        {
            ModelState.AddModelError(string.Empty, "Passwords do not match.");
            return View();
        }

        return RedirectToAction(nameof(Login));
    }
}