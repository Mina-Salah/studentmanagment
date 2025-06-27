using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Services.Interfaces;
using System.Security.Claims;

namespace StudentManagement.Web.Controllers.Dashboard
{
    [Authorize(Roles = "Teacher")]
    public class TeacherDashboardController : Controller
    {
        private readonly ICourseVideoService _videoService;

        public TeacherDashboardController(ICourseVideoService videoService)
        {
            _videoService = videoService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var teacherEmail = User.FindFirstValue(ClaimTypes.Name);
            var videos = await _videoService.GetVideosByTeacherEmailAsync(teacherEmail);
            return View(videos);
        }
    }

}
