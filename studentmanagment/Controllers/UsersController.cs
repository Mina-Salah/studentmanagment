using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagement.Core.Interfaces;
using StudentManagement.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(string search, string roleFilter)
        {
            var allUsers = await _unitOfWork.Users.GetAllIncludingDeletedAsync();

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                allUsers = allUsers.Where(u =>
                    (u.FullName != null && u.FullName.ToLower().Contains(search)) ||
                    (u.Email != null && u.Email.ToLower().Contains(search))
                );
            }

            // Apply role filter if provided
            if (!string.IsNullOrEmpty(roleFilter))
            {
                allUsers = allUsers.Where(u => u.Role == roleFilter);
            }

            var userList = allUsers.ToList();

            var model = new UserFilterViewModel
            {
                Search = search,
                RoleFilter = roleFilter,
                Users = userList,

                TotalUsers = userList.Count,
                TotalStudents = userList.Count(u => u.Role == "User"),
                TotalTeachers = userList.Count(u => u.Role == "Teacher"),
                DeletedCount = userList.Count(u => u.IsDeleted),

                Roles = new List<SelectListItem>
                {
                    new SelectListItem { Text = "جميع الصلاحيات", Value = "" },
                    new SelectListItem { Text = "طالب", Value = "User" },
                    new SelectListItem { Text = "مدرس", Value = "Teacher" },
                    new SelectListItem { Text = "مدير", Value = "Admin" }
                }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsDeleted = true;
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsDeleted = false;
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
