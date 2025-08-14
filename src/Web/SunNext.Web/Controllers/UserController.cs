using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Manage.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using SunNext.Common;
using SunNext.Services.Market;
using SunNext.Services.SolarSystem;
using SunNext.Services.User;
using SunNext.Web.ViewModels.User;

namespace SunNext.Web.Controllers;

public class UserController : BaseController
{
    private readonly ISolarSystemService _solarSystemService;
    private readonly IMarketService _marketService;
    private readonly IUserService _userService;
    

    public UserController( ISolarSystemService solarSystemService, IMarketService _marketService, IUserService _userService )
    {
        this._solarSystemService = solarSystemService;
        this._marketService = _marketService;
        this._userService = _userService;
    }
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public async Task<IActionResult> AdminCockpit()
    {
        var model = new AdminDashboardViewModel
        {
            TotalUsers = await this._userService.CountAsync(),
            TotalSystems = await this._solarSystemService.CountAsync(),
            TradeRecords = await this._marketService.CountAsync(),
        };

        return View("Admin/Cockpit", model);
    }

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
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