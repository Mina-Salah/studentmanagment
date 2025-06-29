using Microsoft.EntityFrameworkCore;
using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces;
using StudentManagement.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateStudentAsync(Student student, List<int> subjectIds, string email, string password)
        {
            // إنشاء المستخدم أولاً
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User
            {
                Email = email,
                PasswordHash = hashedPassword,
                Role = "Student",
                IsActive = true,
                FullName = student.Name
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync(); // حفظ للحصول على Id

            // ربط الطالب بالمستخدم
            student.UserId = user.Id;
            student.StudentSubjects = new List<StudentSubject>();

            var subjects = await _unitOfWork.Subjects.GetAllAsync(sub => subjectIds.Contains(sub.Id));
            foreach (var subject in subjects)
            {
                student.StudentSubjects.Add(new StudentSubject
                {
                    Student = student,
                    Subject = subject
                });
            }

            await _unitOfWork.Students.AddAsync(student);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _unitOfWork.Students.GetAllAsync(
                filter: s => !s.IsDeleted,
                include: q => q
                    .Include(s => s.User) 
                    .Include(s => s.StudentSubjects)
                        .ThenInclude(ss => ss.Subject)
            );
        }


        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _unitOfWork.Students.GetByIdAsync(id,
                include: q => q.Include(s => s.StudentSubjects)
                               .ThenInclude(ss => ss.Subject));
        }

        public async Task UpdateStudentAsync(int id, Student student, List<int> subjectIds)
        {
            var existingStudent = await _unitOfWork.Students.GetByIdAsync(id,
                include: q => q.Include(s => s.StudentSubjects));

            if (existingStudent == null)
                throw new Exception("Student not found");

            existingStudent.Name = student.Name;
            existingStudent.Address = student.Address;
            existingStudent.DateOfBirth = student.DateOfBirth;

            existingStudent.StudentSubjects.Clear();
            var subjects = await _unitOfWork.Subjects.GetAllAsync(sub => subjectIds.Contains(sub.Id));
            foreach (var subject in subjects)
            {
                existingStudent.StudentSubjects.Add(new StudentSubject
                {
                    StudentId = existingStudent.Id,
                    SubjectId = subject.Id
                });
            }

            _unitOfWork.Students.Update(existingStudent);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteStudentAsync(int id)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
                throw new Exception("Student not found");

            student.IsDeleted = true;
            _unitOfWork.Students.Update(student);

            if (student.UserId.HasValue)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(student.UserId.Value);
                if (user != null)
                {
                    user.IsDeleted = true;
                    _unitOfWork.Users.Update(user);
                }
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<Student>> GetDeletedStudentsAsync()
        {
            return await _unitOfWork.Students.GetAllAsync(
                filter: s => s.IsDeleted,
                include: q => q.Include(s => s.StudentSubjects)
                               .ThenInclude(ss => ss.Subject),
                includeSoftDeleted: true
            );
        }

        public async Task RestoreStudentAsync(int id)
        {
            var student = await _unitOfWork.Students.GetByIdIncludingDeletedAsync(id);
            if (student == null)
                throw new Exception("Student not found");

            student.IsDeleted = false;
            _unitOfWork.Students.Update(student);

            if (student.UserId.HasValue)
            {
                var user = await _unitOfWork.Users.GetByIdIncludingDeletedAsync(student.UserId.Value);
                if (user != null)
                {
                    user.IsDeleted = false;
                    _unitOfWork.Users.Update(user);
                }
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteStudentPermanentlyAsync(int id)
        {
            var student = await _unitOfWork.Students.GetByIdIncludingDeletedAsync(id,
                include: q => q.Include(s => s.StudentSubjects));

            if (student == null)
                throw new Exception("Student not found");

            foreach (var ss in student.StudentSubjects.ToList())
            {
                _unitOfWork.StudentSubjects.Delete(ss);
            }

            _unitOfWork.Students.Delete(student);

            if (student.UserId.HasValue)
            {
                var user = await _unitOfWork.Users.GetByIdIncludingDeletedAsync(student.UserId.Value);
                if (user != null)
                {
                    _unitOfWork.Users.Delete(user);
                }
            }

            await _unitOfWork.CompleteAsync();
        }
        public async Task<Student?> GetStudentByEmailAsync(string email)
        {
            return await _unitOfWork.Students.GetFirstOrDefaultAsync(
                filter: s => s.User != null && s.User.Email == email && !s.IsDeleted,
                include: q => q.Include(s => s.User).Include(s => s.StudentSubjects).ThenInclude(ss => ss.Subject)
            );
        }


    }
}
