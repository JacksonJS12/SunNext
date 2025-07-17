using Microsoft.AspNetCore.Mvc;

namespace SunNext.Web.Controllers;

public class TradeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Targets()
    {
        return View();
    }
}