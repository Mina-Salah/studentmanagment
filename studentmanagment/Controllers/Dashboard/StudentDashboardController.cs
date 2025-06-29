using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces;
using StudentManagement.Services.Interfaces; // لو موجودة الخدمة

namespace StudentManagement.Web.Controllers.Dashboard
{
    [Authorize(Roles = "Student,Admin")]
    public class StudentDashboardController : Controller
    {
        private readonly IStudentService _studentService;

        public StudentDashboardController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            // فرضًا عندك دالة في الخدمة تجلب الطالب بناءً على الايميل
            var student = await _studentService.GetStudentByEmailAsync(email);

            if (student == null)
            {
                return View(new Student { Name = "Student" });
            }

            return View(student);
        }
    }
}
