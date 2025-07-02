using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Core.Entities.Course_file;
using StudentManagement.Services.Interfaces;
using StudentManagement.Web.ViewModels;
using System.Security.Claims;

namespace StudentManagement.Web.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class CourseVideoController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ICourseVideoService _videoService;
        private readonly IWebHostEnvironment _env;
        private readonly ITeacherService _teacherService;

        public CourseVideoController(
            ICourseService courseService,
            ICourseVideoService videoService,
            ITeacherService teacherService,
            IWebHostEnvironment env)
        {
            _courseService = courseService;
            _videoService = videoService;
            _teacherService = teacherService;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            var allCourses = await _courseService.GetAllCoursesAsync();

            ViewBag.Courses = allCourses.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Title
            }).ToList();

            return View(new UploadVideoViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Upload(UploadVideoViewModel model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var teacher = await _teacherService.GetTeacherByEmailAsync(email);

            if (teacher == null)
            {
                TempData["Error"] = "لم يتم العثور على المدرس.";
                return RedirectToAction("Login", "Auth");
            }

            var allCourses = await _courseService.GetAllCoursesAsync();
            ViewBag.Courses = allCourses.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Title
            }).ToList();

            if (!ModelState.IsValid)
                return View(model);

            var courseFolder = Path.Combine(_env.WebRootPath, "videos", $"Course_{model.CourseId}");
            if (!Directory.Exists(courseFolder))
                Directory.CreateDirectory(courseFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(model.VideoFile.FileName);
            var filePath = Path.Combine(courseFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.VideoFile.CopyToAsync(stream);
            }

            var video = new CourseVideo
            {
                Title = model.Title,
                VideoPath = $"/videos/Course_{model.CourseId}/{fileName}",
                CourseId = model.CourseId,
                TeacherId = teacher.Id,
                TeacherEmail = email
            };

            await _videoService.AddVideoAsync(video);

            TempData["Success"] = "تم رفع الفيديو بنجاح.";
            return RedirectToAction("MyVideos");
        }

        [HttpGet]
        public async Task<IActionResult> MyVideos()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var videos = await _videoService.GetVideosByTeacherEmailAsync(email);
            return View(videos);
        }
    }
}
