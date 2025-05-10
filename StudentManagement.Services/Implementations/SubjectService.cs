// SubjectService.cs
using Microsoft.EntityFrameworkCore;
using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces;
using StudentManagement.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagement.Services.Implementations
{
    public class SubjectService : ISubjectService
    {
        private readonly IUnitOfWork _unitOfWork;

        // Constructor initializes the service with UnitOfWork.
        public SubjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Retrieves all subjects from the database.
        public async Task<IEnumerable<Subject>> GetAllSubjectsAsync()
        {
            return await _unitOfWork.Subjects.GetAllAsync();
        }

        // Retrieves a subject by its ID.
        public async Task<Subject?> GetSubjectByIdAsync(int id)
        {
            return await _unitOfWork.Subjects.GetByIdAsync(id);
        }

        // Adds a new subject to the database.
        public async Task AddSubjectAsync(Subject subject)
        {
            await _unitOfWork.Subjects.AddAsync(subject);
            await _unitOfWork.CompleteAsync(); 
        }

        // Updates an existing subject.
        public async Task UpdateSubjectAsync(Subject subject)
        {
            _unitOfWork.Subjects.Update(subject);
            await _unitOfWork.CompleteAsync(); 
        }

        // Soft deletes a subject if it's not associated with students.
        public async Task DeleteSubjectAsync(int id)
        {
            var subject = await _unitOfWork.Subjects.GetByIdAsync(id, q =>
                q.Include(s => s.StudentSubjects));

            if (subject == null)
                throw new Exception("Subject not found");

            if (subject.StudentSubjects != null && subject.StudentSubjects.Any())
                throw new InvalidOperationException("Cannot delete this subject because it is linked to students.");

            subject.IsDeleted = true; // Soft deleted.
            _unitOfWork.Subjects.Update(subject);
            await _unitOfWork.CompleteAsync(); // Save changes.
        }
    }
}
