using StudentManagement.Core.Entities;
using StudentManagement.Core.Entities.Course_file;
using StudentManagement.Core.Interfaces;
using StudentManagement.Data.Repositories;
using System;

namespace StudentManagement.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ManagDbContext _context;

        public UnitOfWork(ManagDbContext context)
        {
            _context = context;
            Students = new GenericRepository<Student>(_context);
            Subjects = new GenericRepository<Subject>(_context);
            StudentSubjects = new GenericRepository<StudentSubject>(_context);
            Users = new GenericRepository<User>(_context);
            Courses = new GenericRepository<Course>(_context);
            Categories = new GenericRepository<Category>(_context);
            Teachers = new GenericRepository<Teacher>(_context);
            CourseVideos = new GenericRepository<CourseVideo>(_context); // ✅
            VideoAccesses = new GenericRepository<VideoAccess>(_context); // ✅
        }

        public IGenericRepository<Student> Students { get; private set; }
        public IGenericRepository<Subject> Subjects { get; private set; }
        public IGenericRepository<StudentSubject> StudentSubjects { get; private set; }
        public IGenericRepository<User> Users { get; private set; } 
        public IGenericRepository<Course> Courses { get; }
        public IGenericRepository<CourseVideo> CourseVideos { get; private set; }
        public IGenericRepository<VideoAccess> VideoAccesses { get; private set; }

        public IGenericRepository<Category> Categories { get; private set; }
        public IGenericRepository<Teacher> Teachers { get; private set; }


        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}