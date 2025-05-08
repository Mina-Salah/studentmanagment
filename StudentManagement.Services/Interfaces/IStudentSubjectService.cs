using StudentManagement.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Services.Interfaces
{
    public interface IStudentSubjectService
    {
        Task<IEnumerable<StudentSubject>> GetAllAsync();
        Task<StudentSubject> GetByIdsAsync(int studentId, int subjectId);
        Task AddAsync(StudentSubject studentSubject);
        Task DeleteAsync(int studentId, int subjectId);
    }
}
