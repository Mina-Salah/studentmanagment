using Microsoft.EntityFrameworkCore;
using StudentManagement.Core.Entities.Course_file;
using StudentManagement.Core.Interfaces;
using StudentManagement.Services.Interfaces;
using StudentManagement.Web.ViewModels;

namespace StudentManagement.Services.Implementations
{
    public class CourseVideoService : ICourseVideoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseVideoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CourseVideo>> GetVideosByTeacherEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return new List<CourseVideo>();

            var videos = await _unitOfWork.CourseVideos.GetAllAsync(
                filter: v => v.Teacher != null && v.Teacher.User != null && v.Teacher.User.Email == email,
                include: q => q
                    .Include(v => v.Teacher).ThenInclude(t => t.User)
                    .Include(v => v.Course)
            );

            return videos.ToList();
        }

        public async Task UploadVideoAsync(UploadVideoViewModel model, string teacherEmail, string webRootPath)
        {
            // ✅ تحقق من المدرس
            var teacher = await _unitOfWork.Teachers.GetFirstOrDefaultAsync(
                t => t.User != null && t.User.Email == teacherEmail,
                include: q => q.Include(t => t.User)
            );
            if (teacher == null)
                throw new Exception("لم يتم العثور على المدرس");

            // ✅ تحقق من الكورس
            var course = await _unitOfWork.Courses.GetFirstOrDefaultAsync(c => c.Id == model.CourseId);
            if (course == null)
                throw new Exception("لم يتم العثور على الكورس");

            // ✅ إنشاء مجلد باسم الكورس
            var courseFolderName = course.Title.Replace(" ", "_"); // إزالة المسافات
            var uploadsDir = Path.Combine(webRootPath, "videos", courseFolderName);
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            // ✅ حفظ الفيديو داخل المجلد
            var fileName = Guid.NewGuid() + Path.GetExtension(model.VideoFile.FileName);
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.VideoFile.CopyToAsync(stream);
            }

            // ✅ حفظ بيانات الفيديو في قاعدة البيانات
            var video = new CourseVideo
            {
                Title = model.Title,
                VideoPath = $"/videos/{courseFolderName}/{fileName}",
                TeacherEmail = teacherEmail,
                TeacherId = teacher.Id,
                CourseId = course.Id
            };

            await _unitOfWork.CourseVideos.AddAsync(video);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<List<Course>> GetCoursesByTeacherEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return new List<Course>();

            var courses = await _unitOfWork.Courses.GetAllAsync(
                filter: c => c.CourseTeachers.Any(ct => ct.Teacher.User != null && ct.Teacher.User.Email == email),
                include: q => q
                    .Include(c => c.CourseTeachers)
                        .ThenInclude(ct => ct.Teacher)
                            .ThenInclude(t => t.User)
            );

            return courses.ToList();
        }

        public async Task<int> GetVideoCountByTeacherAsync(string email)
        {
            var videos = await _unitOfWork.CourseVideos.GetAllAsync(
                filter: v => v.Teacher != null && v.Teacher.User != null && v.Teacher.User.Email == email && !v.IsDeleted,
                include: q => q
                    .Include(v => v.Teacher)
                        .ThenInclude(t => t.User)
            );

            return videos.Count();
        }

        public async Task AddVideoAsync(CourseVideo video)
        {
            await _unitOfWork.CourseVideos.AddAsync(video);
            await _unitOfWork.CompleteAsync();
        }
    }
}
