using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces;
using StudentManagement.Data.Contextt;
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
        }

        public IGenericRepository<Student> Students { get; private set; }
        public IGenericRepository<Subject> Subjects { get; private set; }
        public IGenericRepository<StudentSubject> StudentSubjects { get; private set; }

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