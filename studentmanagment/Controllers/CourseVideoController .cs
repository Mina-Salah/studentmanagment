using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Services.Interfaces;
using StudentManagement.Web.ViewModels;
using System.Security.Claims;
using StudentManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace StudentManagement.Web.Controllers
{
    [Authorize(Roles = "Teacher,Admin")]
    public class CourseVideoController : Controller
    {
        private readonly ICourseVideoService _videoService;
        private readonly ICourseService _courseService;

        public CourseVideoController(ICourseVideoService videoService, ICourseService courseService)
        {
            _videoService = videoService;
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name;
            var courses = await _courseService.GetCoursesByTeacherEmailAsync(email);

            var model = new UploadVideoViewModel
            {
                Courses = courses // مباشرة بدون تحويل
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Upload(UploadVideoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var email = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name;
                model.Courses = await _courseService.GetCoursesByTeacherEmailAsync(email);

                return View(model);
            }


            var teacherEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name;
            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            await _videoService.UploadVideoAsync(model, teacherEmail, webRootPath);

            TempData["Success"] = "تم رفع الفيديو بنجاح";
            return RedirectToAction("MyVideos");
        }

        [HttpGet]
        public async Task<IActionResult> MyVideos()
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "لم يتم العثور على البريد الإلكتروني. يرجى إعادة تسجيل الدخول.";
                return RedirectToAction("Login", "Auth");
            }

            var videos = await _videoService.GetVideosByTeacherEmailAsync(email);
            return View(videos);
        }
    }
}
