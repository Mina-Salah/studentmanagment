using StudentManagement.Core.Entities;
using System;
using System.Threading.Tasks;

namespace StudentManagement.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Student> Students { get; }
        IGenericRepository<Subject> Subjects { get; }
        IGenericRepository<StudentSubject> StudentSubjects { get; }
        IGenericRepository<User> Users { get; }
        Task<int> CompleteAsync(); // Equivalent to SaveChangesAsync
    }
}
