using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces;
using StudentManagement.Web.ViewModels;


namespace StudentManagement.Web.Controllers.Dashboard
{
    [Authorize(Roles = "Admin")]

    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                TotalStudents = (await _unitOfWork.Users.GetAllAsync(u => u.Role == "User")).Count(),
                TotalTeachers = (await _unitOfWork.Users.GetAllAsync(u => u.Role == "Teacher")).Count(),
                TotalCourses = (await _unitOfWork.Courses.GetAllAsync()).Count(),
                TotalSubjects = (await _unitOfWork.Subjects.GetAllAsync()).Count()
            };

            return View(model);
        }
    }
}
