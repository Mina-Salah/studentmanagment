using StudentManagement.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagement.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllStudentsAsync();
        Task<bool> IsEmailTakenAsync(string email);
    }
}
