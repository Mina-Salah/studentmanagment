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

            var allVideos = await _unitOfWork.CourseVideos.GetAllAsync();
            return allVideos
                .Where(v => v.TeacherEmail == email)
                .ToList();
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

            var video = new CourseVideo
            {
                Title = model.Title,
                VideoPath = "/videos/" + fileName,
                TeacherEmail = teacherEmail
            };

            await _unitOfWork.CourseVideos.AddAsync(video);
            await _unitOfWork.CompleteAsync();
        }
    }
}
