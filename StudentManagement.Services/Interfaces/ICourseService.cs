using StudentManagement.Core.Entities.Course_file;

namespace StudentManagement.Services.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int id);
        Task AddCourseAsync(Course course);
        Task UpdateCourseAsync(Course course);
        Task DeleteCourseAsync(int id);

        // ✅ كورسات المدرس
        Task<List<Course>> GetCoursesByTeacherEmailAsync(string email);
        Task<List<Course>> GetCoursesByTeacherIdAsync(int teacherId); // أضف هذا إن لم يكن موجودًا
    }
}
