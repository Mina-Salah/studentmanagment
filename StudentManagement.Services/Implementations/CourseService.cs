using Microsoft.EntityFrameworkCore;
using StudentManagement.Core.Entities.Course_file;
using StudentManagement.Core.Interfaces;
using StudentManagement.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return (await _unitOfWork.Courses.GetAllAsync(
     filter: c => !c.IsDeleted,
     include: q => q.Include(c => c.Teacher)
                    .Include(c => c.Category)
 )).ToList();
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _unitOfWork.Courses.GetByIdAsync(id, q =>
                q.Include(c => c.Teacher)
                 .Include(c => c.Category));
        }

        public async Task AddCourseAsync(Course course)
        {
            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateCourseAsync(Course course)
        {
            _unitOfWork.Courses.Update(course);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteCourseAsync(int id)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course != null)
            {
                course.IsDeleted = true;
                _unitOfWork.Courses.Update(course);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<List<Course>> GetCoursesByTeacherEmailAsync(string email)
        {
            var courses = await _unitOfWork.Courses.GetAllAsync(
                filter: c => c.Teacher != null && c.Teacher.Email == email,
                include: q => q.Include(c => c.Teacher)
            );

            return courses.ToList();
        }
    }
}
