using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Core.Interfaces;
using StudentManagement.Web.ViewModels;

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
        var allUsers = await _unitOfWork.Users.GetAllIncludingDeletedAsync();
        var activeUsers = allUsers.Where(u => !u.IsDeleted).ToList();
        var deletedUsers = allUsers.Where(u => u.IsDeleted).ToList();

        var model = new DashboardViewModel
        {
            TotalUsers = allUsers.Count(),
            ActiveUsers = activeUsers.Count(),
            DeletedUsers = deletedUsers.Count(),

            TotalStudents = allUsers.Count(u => u.Role == "Student"),
            ActiveStudents = activeUsers.Count(u => u.Role == "Student"),
            DeletedStudents = deletedUsers.Count(u => u.Role == "Student"),

            TotalTeachers = allUsers.Count(u => u.Role == "Teacher"),
            ActiveTeachers = activeUsers.Count(u => u.Role == "Teacher"),
            DeletedTeachers = deletedUsers.Count(u => u.Role == "Teacher"),

            TotalCourses = (await _unitOfWork.Courses.GetAllAsync()).Count(),
            TotalSubjects = (await _unitOfWork.Subjects.GetAllAsync()).Count()
        };

        return View(model);
    }
}
