using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                allUsers = allUsers.Where(u =>
                    (u.FullName != null && u.FullName.ToLower().Contains(search)) ||
                    (u.Email != null && u.Email.ToLower().Contains(search))
                );
            }

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
                TotalStudents = userList.Count(u => u.Role == "Student"),
                TotalTeachers = userList.Count(u => u.Role == "Teacher"),
                DeletedCount = userList.Count(u => u.IsDeleted),
                Roles = new List<SelectListItem>
                {
                    new SelectListItem { Text = "جميع الصلاحيات", Value = "" },
                    new SelectListItem { Text = "طالب", Value = "Student" },
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
            var user = await _unitOfWork.Users.GetByIdIncludingDeletedAsync(id, q =>
                  q.Include(u => u.Student)
                   .Include(u => u.Teacher)
              ); if (user == null)
                return NotFound();

            user.IsDeleted = true;

            // حذف الطالب أو المدرس المرتبط
            if (user.Role == "Student" && user.Student != null)
                user.Student.IsDeleted = true;

            if (user.Role == "Teacher" && user.Teacher != null)
                user.Teacher.IsDeleted = true;

            await _unitOfWork.CompleteAsync();

            TempData["Message"] = $"تم حذف المستخدم \"{user.FullName}\" مؤقتًا.";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var user = await _unitOfWork.Users.GetByIdIncludingDeletedAsync(id, q =>
                  q.Include(u => u.Student)
                   .Include(u => u.Teacher)
              );
            if (user == null)
                return NotFound();

            user.IsDeleted = false;

            // استرجاع الطالب أو المدرس المرتبط
            if (user.Role == "Student" && user.Student != null)
                user.Student.IsDeleted = false;

            if (user.Role == "Teacher" && user.Teacher != null)
                user.Teacher.IsDeleted = false;

            await _unitOfWork.CompleteAsync();

            TempData["Message"] = $"تم استرجاع المستخدم \"{user.FullName}\" بنجاح.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HardDelete(int id)
        {
            var user = await _unitOfWork.Users.GetByIdIncludingDeletedAsync(id, q =>
                  q.Include(u => u.Student)
                   .Include(u => u.Teacher)
              );
            if (user == null)
                return NotFound();

            // حذف الطالب أو المدرس المرتبط أولًا
            if (user.Role == "Student" && user.Student != null)
                _unitOfWork.Students.Delete(user.Student);

            if (user.Role == "Teacher" && user.Teacher != null)
                _unitOfWork.Teachers.Delete(user.Teacher);

            _unitOfWork.Users.Delete(user);
            await _unitOfWork.CompleteAsync();

            TempData["Message"] = $"تم حذف المستخدم \"{user.FullName}\" نهائيًا.";
            return RedirectToAction(nameof(Index));
        }

    }
}
