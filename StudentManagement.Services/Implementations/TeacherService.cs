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
    public class TeacherService : ITeacherService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TeacherService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
        {
            return await _unitOfWork.Teachers.GetAllAsync();
        }

        public async Task<Teacher?> GetTeacherByIdAsync(int id)
        {
            return await _unitOfWork.Teachers.GetByIdAsync(id);
        }

        public async Task AddTeacherAsync(Teacher teacher)
        {
            await _unitOfWork.Teachers.AddAsync(teacher);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateTeacherAsync(Teacher teacher)
        {
            _unitOfWork.Teachers.Update(teacher);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteTeacherAsync(int id)
        {
            var teacher = await _unitOfWork.Teachers.GetByIdAsync(id);
            if (teacher != null)
            {
                _unitOfWork.Teachers.Delete(teacher);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}
