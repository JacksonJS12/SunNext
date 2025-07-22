using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SunNext.Web.Controllers;

public class MonitoringController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}