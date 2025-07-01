using StudentManagement.Core.Entities.Course_file;
using StudentManagement.Web.ViewModels;

namespace StudentManagement.Services.Interfaces
{
    public interface ICourseVideoService
    {
        Task<List<CourseVideo>> GetVideosByTeacherEmailAsync(string email);
        Task<int> GetVideoCountByTeacherAsync(string email);

        Task<List<Course>> GetCoursesByTeacherEmailAsync(string email);

        Task UploadVideoAsync(UploadVideoViewModel model, string teacherEmail, string webRootPath);
    }
}
