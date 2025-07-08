using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces;
using StudentManagement.Services.Interfaces;
using StudentManagement.Web.ViewModels;

namespace StudentManagement.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TeachersController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TeachersController(
            ITeacherService teacherService,
            IUserService userService,
            IAuthService authService,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _teacherService = teacherService;
            _userService = userService;
            _authService = authService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var teachers = await _teacherService.GetAllTeachersAsync();
            var viewModels = _mapper.Map<List<TeacherViewModel>>(teachers);
            return View(viewModels);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Password))
            {
                if (string.IsNullOrWhiteSpace(model.Password))
                    ModelState.AddModelError("Password", "كلمة المرور مطلوبة.");

                return View(model);
            }

            if (await _userService.IsEmailTakenAsync(model.Email))
            {
                ModelState.AddModelError("Email", "هذا الإيميل مستخدم بالفعل.");
                return View(model);
            }

            var success = await _authService.RegisterAsync(model.Email, model.Password, "Teacher", model.Name);
            if (!success)
            {
                ModelState.AddModelError("", "فشل في إنشاء الحساب.");
                return View(model);
            }

            var user = await _authService.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "لم يتم العثور على المستخدم بعد التسجيل.");
                return View(model);
            }

            var teacher = _mapper.Map<Teacher>(model);
            teacher.UserId = user.Id;

            await _teacherService.CreateTeacherAsync(teacher, model.Email, model.Password);

            TempData["SuccessMessage"] = $"تم إنشاء المدرس بنجاح. البريد: {model.Email}، كلمة السر: {model.Password}";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null) return NotFound();

            var viewModel = _mapper.Map<TeacherViewModel>(teacher);

            if (teacher.User != null)
                viewModel.Email = teacher.User.Email;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TeacherViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingTeacher = await _teacherService.GetTeacherByIdAsync(model.Id);
            if (existingTeacher == null) return NotFound();

            var user = existingTeacher.User;

            // التحقق من تكرار الإيميل فقط لو تم تغييره
            if (user != null && user.Email.ToLower() != model.Email.ToLower() &&
                await _userService.IsEmailTakenAsync(model.Email))
            {
                ModelState.AddModelError("Email", "هذا الإيميل مستخدم بالفعل.");
                return View(model);
            }

            // تحديث بيانات المدرس
            existingTeacher.Name = model.Name;
            existingTeacher.Address = model.Address;
            existingTeacher.DateOfBirth = model.DateOfBirth;

            await _teacherService.UpdateTeacherAsync(model.Id, existingTeacher);

            // تحديث بيانات المستخدم المرتبط
            if (user != null)
            {
                user.Email = model.Email;
                user.FullName = model.Name;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();
            }

            TempData["SuccessMessage"] = "تم تحديث بيانات المدرس بنجاح.";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _teacherService.DeleteTeacherAsync(id);
                TempData["SuccessMessage"] = "تم حذف المدرس بنجاح.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"حدث خطأ أثناء الحذف: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Deleted()
        {
            var deletedTeachers = await _teacherService.GetDeletedTeachersAsync();
            var viewModels = _mapper.Map<List<TeacherViewModel>>(deletedTeachers);
            return View(viewModels);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                await _teacherService.RestoreTeacherAsync(id);
                TempData["SuccessMessage"] = "تم استرجاع المدرس بنجاح.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"حدث خطأ أثناء الاسترجاع: {ex.Message}";
            }

            return RedirectToAction(nameof(Deleted));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePermanent(int id)
        {
            try
            {
                await _teacherService.DeleteTeacherPermanentlyAsync(id);
                TempData["SuccessMessage"] = "تم حذف المدرس نهائيًا.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"حدث خطأ أثناء الحذف النهائي: {ex.Message}";
            }

            return RedirectToAction(nameof(Deleted));
        }
    }
}
