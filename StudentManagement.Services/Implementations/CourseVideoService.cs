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
                filter: v => v.Teacher.Email == email,
                include: q => q.Include(v => v.Teacher)
            );

            return videos.ToList();
        }


        public async Task UploadVideoAsync(UploadVideoViewModel model, string teacherEmail, string webRootPath)
        {
            var uploadsDir = Path.Combine(webRootPath, "videos");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var fileName = Guid.NewGuid() + Path.GetExtension(model.VideoFile.FileName);
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.VideoFile.CopyToAsync(stream);
            }

            // 🟢 نجيب المدرس عشان نحدد الـ TeacherId
            var teacher = await _unitOfWork.Teachers.GetFirstOrDefaultAsync(t => t.Email == teacherEmail);
            if (teacher == null)
                throw new Exception("لم يتم العثور على المدرس");

            var video = new CourseVideo
            {
                Title = model.Title,
                VideoPath = "/videos/" + fileName,
                TeacherEmail = teacherEmail,
                TeacherId = teacher.Id,
                CourseId = model.CourseId
            };

            await _unitOfWork.CourseVideos.AddAsync(video);
            await _unitOfWork.CompleteAsync();
        }
        public async Task<List<Course>> GetCoursesByTeacherEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return new List<Course>();

            var courses = await _unitOfWork.Courses.GetAllAsync(
                filter: c => c.Teacher != null && c.Teacher.Email == email,
                include: q => q.Include(c => c.Teacher)
            );

            return courses.ToList();
        }


        public async Task<int> GetVideoCountByTeacherAsync(string email)
        {
            var videos = await _unitOfWork.CourseVideos.GetAllAsync(
                filter: v => v.Teacher.Email == email && !v.IsDeleted,
                include: q => q.Include(v => v.Teacher)
            );

            return videos.Count();
        }


    }
}
