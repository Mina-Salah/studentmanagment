using StudentManagement.Core.Entities;

namespace StudentManagement.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<IEnumerable<Subject>> GetAllSubjectsAsync();
        Task<Subject?> GetSubjectByIdAsync(int id);
        Task AddSubjectAsync(Subject subject);
        Task UpdateSubjectAsync(Subject subject);
        Task DeleteSubjectAsync(int id);
    }
}
