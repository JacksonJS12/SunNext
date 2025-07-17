namespace SunNext.Web.Areas.Administration.Controllers
{
    using SunNext.Web.ViewModels.Administration.Dashboard;

    using Microsoft.AspNetCore.Mvc;

    public class DashboardController : AdministrationController
    {

        public DashboardController()
        {
        }

        public IActionResult Index()
        {
            var viewModel = new IndexViewModel {};
            return this.View(viewModel);
        }
        public IActionResult Overview()
        {
            return View();
        }
        public IActionResult Users()
        {
            return View();
        }
    }
}
