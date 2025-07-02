using StudentManagement.Core.Entities;
using StudentManagement.Core.Entities.Course_file;
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
        IGenericRepository<Course> Courses { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Teacher> Teachers { get; }
        IGenericRepository<CourseVideo> CourseVideos { get; } 
        IGenericRepository<VideoAccess> VideoAccesses { get; }
        IGenericRepository<CourseTeacher> CourseTeachers { get; }

        Task<int> CompleteAsync(); // Equivalent to SaveChangesAsync
    }
}
