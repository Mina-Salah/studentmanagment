using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudentManagement.Web.Controllers.Dashboard
{
    [Authorize(Roles = "Student,Admin")]

    public class StudentDashboardController : Controller
    {
        public IActionResult Dashboard() => View();
    }

}
