﻿

using BCrypt.Net;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces;
using StudentManagement.Services.Interfaces;

namespace StudentManagement.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        private const string AdminEmail = "Mina@gmail.com";
        private const string AdminPassword = "Admin1412";

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> RegisterAsync(string email, string password, string role,string fullname)
        {
            var hashed = HashPassword(password);

            var existingUser = (await _unitOfWork.Users.GetAllAsync())
                .FirstOrDefault(u => u.Email == email);

            if (existingUser != null)
                return false;

            var user = new User
            {
                Email = email,
                PasswordHash = hashed,
                Role = role,
                IsActive = true,
                FullName= fullname
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync(); // حفظ التغييرات
            return true;
        }


        public async Task<User?> LoginAsync(string email, string password)
        {
            if (email == AdminEmail && password == AdminPassword)
            {
                return new User { Email = AdminEmail, Role = "Admin" };
            }

            var users = await _unitOfWork.Users.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Email == email);

            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }

            return null;
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return users.FirstOrDefault(u => u.Email == email);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }

}
