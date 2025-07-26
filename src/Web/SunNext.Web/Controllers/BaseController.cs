using System.Security.Claims;

namespace SunNext.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class BaseController : Controller
    {
        protected string GetUserId()
        {
            var id = string.Empty;

            if (this.User != null)
            {
                id = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            return id;
        }
    }
}
