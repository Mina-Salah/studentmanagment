using Microsoft.AspNetCore.Mvc;

namespace StudentManagement.Web.Controllers.Authorize
{
    public class HomeauthorizationController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Welcome", "Auth");
            }

            return RedirectToAction("Login", "Auth");
        }
    }
}
