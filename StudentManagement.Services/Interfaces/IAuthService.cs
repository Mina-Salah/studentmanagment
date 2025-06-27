

using StudentManagement.Core.Entities;

namespace StudentManagement.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(string email, string password, string role,string fullname);
        Task<User?> LoginAsync(string email, string password );
        Task<User?> GetUserByEmailAsync(string email);

    }

}
