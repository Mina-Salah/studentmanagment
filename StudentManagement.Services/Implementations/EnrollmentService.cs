using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces;
using StudentManagement.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EnrollmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task EnrollStudentInCourseAsync(int studentId, int courseId)
        {
            var isAlreadyEnrolled = await IsStudentEnrolledAsync(studentId, courseId);
            if (isAlreadyEnrolled) return;

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrolledAt = DateTime.UtcNow
            };

            await _unitOfWork.Enrollments.AddAsync(enrollment);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> IsStudentEnrolledAsync(int studentId, int courseId)
        {
            var existing = await _unitOfWork.Enrollments
                .GetFirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            return existing != null;
        }

        public async Task<List<int>> GetEnrolledCourseIdsForStudentAsync(int studentId)
        {
            var enrollments = await _unitOfWork.Enrollments
                .GetAllAsync(e => e.StudentId == studentId);

            return enrollments.Select(e => e.CourseId).ToList();
        }

        public async Task<int> GetEnrollmentCountAsync(int courseId)
        {
            return await _unitOfWork.Enrollments.CountAsync(e => e.CourseId == courseId && !e.IsDeleted);
        }


    }

}
