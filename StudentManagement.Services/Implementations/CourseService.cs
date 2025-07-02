using Microsoft.EntityFrameworkCore;
using StudentManagement.Core.Entities.Course_file;
using StudentManagement.Core.Interfaces;
using StudentManagement.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
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
                include: q => q
                    .Include(c => c.Category)
                    .Include(c => c.CourseTeachers)
                        .ThenInclude(ct => ct.Teacher)
            )).ToList();
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _unitOfWork.Courses.GetByIdAsync(id, q =>
                q.Include(c => c.Category)
                 .Include(c => c.CourseTeachers)
                    .ThenInclude(ct => ct.Teacher));
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

        // ✅ تم تحديثها لتبحث في CourseTeachers بدل Teacher.Email
        public async Task<List<Course>> GetCoursesByTeacherEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return new List<Course>();

            var courses = await _unitOfWork.Courses.GetAllAsync(
                filter: c => c.CourseTeachers.Any(ct =>
                    ct.Teacher != null && ct.Teacher.User != null && ct.Teacher.User.Email == email),
                include: q => q
                    .Include(c => c.CourseTeachers)
                        .ThenInclude(ct => ct.Teacher)
                            .ThenInclude(t => t.User)
            );

            return courses?.ToList() ?? new List<Course>();
        }




        // ✅ جديدة: للبحث باستخدام TeacherId
        public async Task<List<Course>> GetCoursesByTeacherIdAsync(int teacherId)
        {
            var courses = await _unitOfWork.Courses.GetAllAsync(
                filter: c => c.CourseTeachers.Any(ct => ct.TeacherId == teacherId),
                include: q => q
                    .Include(c => c.CourseTeachers)
                        .ThenInclude(ct => ct.Teacher)
                    .Include(c => c.Category)
            );

            return courses.ToList();
        }
    }
}
