using Microsoft.AspNetCore.Mvc;

namespace SunNext.Web.Controllers;

public class SolarAssetController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Create()
    {
        return View();
    }

    public IActionResult Details()
    {
        return View();
    }
}