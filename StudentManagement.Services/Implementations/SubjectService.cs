// SubjectService.cs
using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces;
using StudentManagement.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagement.Servicess.Implementations
{
    public class SubjectService : ISubjectService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Subject>> GetAllSubjectsAsync()
        {
            return await _unitOfWork.Subjects.GetAllAsync();
        }

        public async Task<Subject?> GetSubjectByIdAsync(int id)
        {
            return await _unitOfWork.Subjects.GetByIdAsync(id);
        }

        public async Task AddSubjectAsync(Subject subject)
        {
            await _unitOfWork.Subjects.AddAsync(subject);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateSubjectAsync(Subject subject)
        {
            _unitOfWork.Subjects.Update(subject);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteSubjectAsync(int id)
        {
            var subject = await _unitOfWork.Subjects.GetByIdAsync(id);
            if (subject != null)
            {
                _unitOfWork.Subjects.Delete(subject);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}
