using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces;
using StudentManagement.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<User>> GetAllStudentsAsync()
        {
            var allUsers = await _unitOfWork.Users.GetAllAsync(u => u.Role == "Student");
            return allUsers.ToList();
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            var users = await _unitOfWork.Users.GetAllIncludingDeletedAsync();
            return users.Any(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task SoftDeleteUserAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return;

            user.IsDeleted = true;
            _unitOfWork.Users.Update(user);

            if (user.Role == "Teacher")
            {
                var teacher = (await _unitOfWork.Teachers.GetAllAsync()).FirstOrDefault(t => t.UserId == user.Id);
                if (teacher != null)
                {
                    teacher.IsDeleted = true;
                    _unitOfWork.Teachers.Update(teacher);
                }
            }
            else if (user.Role == "Student")
            {
                var student = (await _unitOfWork.Students.GetAllAsync()).FirstOrDefault(s => s.UserId == user.Id);
                if (student != null)
                {
                    student.IsDeleted = true;
                    _unitOfWork.Students.Update(student);
                }
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task RestoreUserAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdIncludingDeletedAsync(userId);
            if (user == null) return;

            user.IsDeleted = false;
            _unitOfWork.Users.Update(user);

            if (user.Role == "Teacher")
            {
                var teacher = (await _unitOfWork.Teachers.GetAllAsync()).FirstOrDefault(t => t.UserId == user.Id);
                if (teacher != null)
                {
                    teacher.IsDeleted = false;
                    _unitOfWork.Teachers.Update(teacher);
                }
            }
            else if (user.Role == "Student")
            {
                var student = (await _unitOfWork.Students.GetAllAsync()).FirstOrDefault(s => s.UserId == user.Id);
                if (student != null)
                {
                    student.IsDeleted = false;
                    _unitOfWork.Students.Update(student);
                }
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task HardDeleteUserAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdIncludingDeletedAsync(userId);
            if (user == null) return;

            if (user.Role == "Teacher")
            {
                var teacher = (await _unitOfWork.Teachers.GetAllAsync()).FirstOrDefault(t => t.UserId == user.Id);
                if (teacher != null)
                {
                    _unitOfWork.Teachers.Delete(teacher);
                }
            }
            else if (user.Role == "Student")
            {
                var student = (await _unitOfWork.Students.GetAllAsync()).FirstOrDefault(s => s.UserId == user.Id);
                if (student != null)
                {
                    _unitOfWork.Students.Delete(student);
                }
            }

            _unitOfWork.Users.Delete(user);
            await _unitOfWork.CompleteAsync();
        }
    }
}
