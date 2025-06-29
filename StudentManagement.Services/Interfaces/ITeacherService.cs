using StudentManagement.Core.Entities;

namespace StudentManagement.Services.Interfaces
{
    public interface ITeacherService
    {
        Task CreateTeacherAsync(Teacher teacher, string email, string password); // ✅ أضف هذا
        Task<IEnumerable<Teacher>> GetAllTeachersAsync();
        Task<Teacher?> GetTeacherByIdAsync(int id);
        Task UpdateTeacherAsync(int id, Teacher teacher);
        Task DeleteTeacherAsync(int id);
        Task<IEnumerable<Teacher>> GetDeletedTeachersAsync();
        Task RestoreTeacherAsync(int id);
        Task DeleteTeacherPermanentlyAsync(int id);
    }
}
