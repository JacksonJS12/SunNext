using Microsoft.AspNetCore.Mvc;

namespace SunNext.Web.Controllers;

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}