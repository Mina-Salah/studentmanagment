using StudentManagement.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagement.Services.Interfaces
{
    public interface IStudentService
    {
        Task CreateStudentAsync(Student student, List<int> subjectIds);
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByIdAsync(int id);
        Task UpdateStudentAsync(int id, Student student, List<int> subjectIds);
        Task DeleteStudentAsync(int id);
    }
}
