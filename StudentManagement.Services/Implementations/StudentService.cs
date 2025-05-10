using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Services.Interfaces;

namespace StudentManagement.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;

        // Constructor initializes the service with UnitOfWork.
        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Creates a new student and associates selected subjects.
        public async Task CreateStudentAsync(Student student, List<int> subjectIds)
        {
            student.StudentSubjects = new List<StudentSubject>();

            var subjects = await _unitOfWork.Subjects.GetAllAsync(s => s.Where(sub => subjectIds.Contains(sub.Id)));

            foreach (var subject in subjects)
            {
                student.StudentSubjects.Add(new StudentSubject
                {
                    Student = student,
                    Subject = subject
                });
            }

            await _unitOfWork.Students.AddAsync(student);
            await _unitOfWork.CompleteAsync(); // Save changes
        }

        // Retrieves all non-deleted students with their subjects.
        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _unitOfWork.Students.GetAllAsync(q =>
                q.Where(s => !s.IsDeleted)
                 .Include(s => s.StudentSubjects)
                 .ThenInclude(ss => ss.Subject));
        }

        // Retrieves a student by its ID with associated subjects.
        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _unitOfWork.Students.GetByIdAsync(id, q =>
                q.Include(s => s.StudentSubjects).ThenInclude(ss => ss.Subject));
        }

        // Updates student details and subject associations.
        public async Task UpdateStudentAsync(int id, Student student, List<int> subjectIds)
        {
            var existingStudent = await _unitOfWork.Students.GetByIdAsync(id, q =>
                q.Include(s => s.StudentSubjects));

            if (existingStudent == null)
                throw new Exception("Student not found");

            // Update student details
            existingStudent.Name = student.Name;
            existingStudent.Address = student.Address;
            existingStudent.DateOfBirth = student.DateOfBirth;

            // Update subjects
            existingStudent.StudentSubjects.Clear();
            var subjects = await _unitOfWork.Subjects.GetAllAsync(s => s.Where(sub => subjectIds.Contains(sub.Id)));
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

        // Soft deletes a student by marking it as deleted.
        public async Task DeleteStudentAsync(int id)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
                throw new Exception("Student not found");

            student.IsDeleted = true;
            _unitOfWork.Students.Update(student);
            await _unitOfWork.CompleteAsync(); // Save changes
        }

        // Retrieves all deleted students with their subjects.
        public async Task<IEnumerable<Student>> GetDeletedStudentsAsync()
        {
            return await _unitOfWork.Students.GetAllAsync(q =>
                q.Where(s => s.IsDeleted)
                 .Include(s => s.StudentSubjects)
                 .ThenInclude(ss => ss.Subject));
        }

        // Restores a previously deleted student.
        public async Task RestoreStudentAsync(int id)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
                throw new Exception("Student not found");

            student.IsDeleted = false;
            _unitOfWork.Students.Update(student);
            await _unitOfWork.CompleteAsync(); // Save changes
        }
    }
}
