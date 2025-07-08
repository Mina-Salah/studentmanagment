using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Core.Entities;
using StudentManagement.Core.Entities.Course_file;
using StudentManagement.Core.Interfaces;
using StudentManagement.Data.UnitOfWork;
using StudentManagement.Services.Interfaces;
using StudentManagement.Web.ViewModels; 
namespace StudentManagement.Web.Controllers.Dashboard
{
    [Authorize(Roles = "Student,Admin")]
    public class StudentDashboardController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ICourseVideoService _videoService; 
        private readonly IEnrollmentService _enrollmentService;
        private readonly IUnitOfWork _unitOfWork;

        public StudentDashboardController(
      IStudentService studentService,
      ICourseVideoService videoService,
      IEnrollmentService enrollmentService,
      IUnitOfWork unitOfWork) 
        {
            _studentService = studentService;
            _videoService = videoService;
            _enrollmentService = enrollmentService;
            _unitOfWork = unitOfWork; 
        }


        public async Task<IActionResult> Dashboard()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login", "Account");

            var student = await _studentService.GetStudentByEmailAsync(email);
            if (student == null) return Unauthorized();

            var enrolledCourseIds = await _enrollmentService.GetEnrolledCourseIdsForStudentAsync(student.Id);
            var allVideos = await _videoService.GetAllVideosWithCourseAsync();

            var model = new StudentDashboardViewModel
            {
                StudentName = student,
                Courses = new List<CourseProgressViewModel>()
            };

            foreach (var courseId in enrolledCourseIds)
            {
                var courseVideos = allVideos.Where(v => v.CourseId == courseId).ToList();
                var course = courseVideos.FirstOrDefault()?.Course;
                if (course == null) continue;

                var watchedVideos = await _unitOfWork.VideoAccesses.GetAllAsync(
                    x => x.StudentId == student.Id && x.CourseVideo.CourseId == courseId,
                    include: q => q.Include(x => x.CourseVideo)
                );

                int totalWatchedMinutes = watchedVideos.Sum(v => v.CourseVideo?.DurationInMinutes ?? 0);

                model.Courses.Add(new CourseProgressViewModel
                {
                    CourseId = course.Id,
                    Title = course.Title,
                    TotalVideos = courseVideos.Count,
                    WatchedVideosCount = watchedVideos.Count(),
                    TotalWatchedMinutes = totalWatchedMinutes
                });
            }

            return View(model);
        }


        public async Task<IActionResult> AllCourses()
        {
            var videos = await _videoService.GetAllVideosWithCourseAsync();

            var groupedVideos = videos
                .Where(v => v.Course != null)
                .GroupBy(v => v.Course)
                .ToList();

            ViewBag.CourseCount = groupedVideos.Count; 

            return View(groupedVideos);
        }

        [HttpPost]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var email = User.Identity?.Name;
            var student = await _studentService.GetStudentByEmailAsync(email);

            if (student == null)
                return Unauthorized();

            await _enrollmentService.EnrollStudentInCourseAsync(student.Id, courseId);

            return RedirectToAction("AllCourses");
        }
        public async Task<IActionResult> Courses()
        {
            var email = User.Identity?.Name;
            var student = await _studentService.GetStudentByEmailAsync(email);

            if (student == null)
                return Unauthorized();

            // Get IDs of courses the student is enrolled in
            var enrolledCourses = await _enrollmentService.GetEnrolledCourseIdsForStudentAsync(student.Id);

            // Get all videos including courses
            var allVideos = await _videoService.GetAllVideosWithCourseAsync();

            // Filter videos by enrolled courses
            var filteredVideos = allVideos
                .Where(v => v.Course != null && enrolledCourses.Contains(v.Course.Id))
                .GroupBy(v => v.Course)
                .ToList();

            return View(filteredVideos);
        }
        public async Task<IActionResult> CourseVideos(int courseId)
        {
            var email = User.Identity?.Name;
            var student = await _studentService.GetStudentByEmailAsync(email);

            if (student == null)
                return Unauthorized();

            // تحقق إذا كان الطالب مشترك في هذا الكورس
            var isEnrolled = await _enrollmentService.IsStudentEnrolledAsync(student.Id, courseId);
            if (!isEnrolled)
                return Forbid(); // أو Redirect مع رسالة

            // احضر الفيديوهات المرتبطة بالكورس
            var videos = await _videoService.GetVideosByCourseIdAsync(courseId); // لازم تعملها في ICourseVideoService

            // احضر قائمة الفيديوهات التي شاهدها الطالب
            var watchedVideoIds = await _unitOfWork.VideoAccesses.GetAllAsync(
                v => v.StudentId == student.Id && v.CourseVideo.CourseId == courseId
            );

            var model = new VideoCourseViewModel
            {
                CourseId = courseId,
                Videos = videos.OrderBy(v => v.Id).ToList(),
                WatchedVideoIds = watchedVideoIds.Select(v => v.CourseVideoId).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsWatched(int videoId, int courseId)
        {
            var email = User.Identity?.Name;
            var student = await _studentService.GetStudentByEmailAsync(email);

            if (student == null)
                return Unauthorized();

            var alreadyWatched = await _unitOfWork.VideoAccesses.GetFirstOrDefaultAsync(
                v => v.StudentId == student.Id && v.CourseVideoId == videoId
            );

            if (alreadyWatched == null)
            {
                await _unitOfWork.VideoAccesses.AddAsync(new VideoAccess
                {
                    StudentId = student.Id,
                    CourseVideoId = videoId,
                    WatchedAt = DateTime.UtcNow
                });

                await _unitOfWork.CompleteAsync();
            }

            return Ok(new { message = "تم تسجيل المشاهدة" });
        }


    }
}
