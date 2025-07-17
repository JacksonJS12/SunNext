using System;

namespace SunNext.Web.Controllers
{
    using System.Diagnostics;
    using SunNext.Web.ViewModels;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Contact()
        {
            return this.View();
        }

        public IActionResult About()
        {
            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }
        [Route("Error/404")]
        public IActionResult Error404() => View("Error404");

        [Route("Error/500")]
        public IActionResult Error500()
        {
           return View("Error500");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
        public IActionResult TriggerError()
        {
            throw new Exception("Simulated internal server error.");
        }

    }
}