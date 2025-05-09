

using StudentManagement.Core.Entities;

namespace StudentManagement.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(string email, string password);
        Task<User?> LoginAsync(string email, string password);
    }

}
