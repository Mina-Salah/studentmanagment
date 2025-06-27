using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Services.Interfaces;
using StudentManagement.Web.ViewModels;
using System.Security.Claims;

namespace StudentManagement.Web.Controllers
{
    [Authorize(Roles = "Teacher,Admin")]
    public class CourseVideoController : Controller
    {
        private readonly ICourseVideoService _videoService;

        public CourseVideoController(ICourseVideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View(new UploadVideoViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Upload(UploadVideoViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "لم يتم العثور على البريد الإلكتروني للمستخدم. يرجى إعادة تسجيل الدخول.";
                return RedirectToAction("Login", "Auth");
            }

            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            await _videoService.UploadVideoAsync(model, email, webRootPath);

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
