using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagement.Core.Entities.Course_file;
using StudentManagement.Services.Interfaces;
using StudentManagement.Web.ViewModels;
using System.Security.Claims;
using MediaToolkit;
using MediaToolkit.Model;

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
            var email = User.FindFirstValue(ClaimTypes.Email);
            var teacher = await _teacherService.GetTeacherByEmailAsync(email);

            if (teacher == null)
            {
                TempData["Error"] = "لم يتم العثور على المدرس.";
                return RedirectToAction("Login", "Auth");
            }

            var teacherCourses = await _courseService.GetCoursesByTeacherIdAsync(teacher.Id);

            ViewBag.Courses = teacherCourses.Select(c => new SelectListItem
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

            var teacherCourses = await _courseService.GetCoursesByTeacherIdAsync(teacher.Id);
            ViewBag.Courses = teacherCourses.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Title
            }).ToList();

            if (!ModelState.IsValid)
                return View(model);

            if (!teacherCourses.Any(c => c.Id == model.CourseId))
            {
                TempData["Error"] = "لا يمكنك رفع فيديو لهذا الكورس.";
                return View(model);
            }

            var course = teacherCourses.First(c => c.Id == model.CourseId);
            var courseFolderName = course.Title.Replace(" ", "_");
            var uploadsDir = Path.Combine(_env.WebRootPath, "videos", courseFolderName);
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var fileName = Guid.NewGuid() + Path.GetExtension(model.VideoFile.FileName);
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.VideoFile.CopyToAsync(stream);
            }

            // ✅ حساب مدة الفيديو باستخدام MediaToolkit
            var inputFile = new MediaFile { Filename = filePath };
            int durationInMinutes = 0;
            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);
                var duration = inputFile.Metadata?.Duration ?? TimeSpan.Zero;
                durationInMinutes = (int)Math.Ceiling(duration.TotalMinutes);
            }

            var video = new CourseVideo
            {
                Title = model.Title,
                VideoPath = $"/videos/{courseFolderName}/{fileName}",
                CourseId = course.Id,
                TeacherId = teacher.Id,
                TeacherEmail = email,
                DurationInMinutes = durationInMinutes
            };

            await _videoService.AddVideoAsync(video);

            TempData["Success"] = "تم رفع الفيديو وحساب المدة بنجاح.";
            return RedirectToAction("MyVideos");
        }

        [HttpGet]
        public async Task<IActionResult> MyVideos()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var videos = await _videoService.GetVideosByTeacherEmailAsync(email);

            var groupedVideos = videos
                .Where(v => v.Course != null)
                .GroupBy(v => v.Course)
                .ToList();

            return View(groupedVideos);
        }
    }
}
