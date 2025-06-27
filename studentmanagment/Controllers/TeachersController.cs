using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Core.Entities;
using StudentManagement.Services.Interfaces;
using StudentManagement.Web.ViewModels;

namespace StudentManagement.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TeachersController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public TeachersController(
            ITeacherService teacherService,
            IAuthService authService,
            IMapper mapper)
        {
            _teacherService = teacherService;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var teachers = await _teacherService.GetAllTeachersAsync();
            var viewModels = _mapper.Map<IEnumerable<TeacherViewModel>>(teachers);
            return View(viewModels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // إنشاء حساب المستخدم للمدرس
            var success = await _authService.RegisterAsync(model.Email, model.Password, "Teacher", model.FullName);

            if (!success)
            {
                ModelState.AddModelError("", "Email is already registered.");
                return View(model);
            }

            // الحصول على المستخدم المرتبط بالإيميل
            var user = await _authService.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Error creating user account.");
                return View(model);
            }

            // ربط المدرس بالمستخدم
            var teacher = _mapper.Map<Teacher>(model);
            teacher.UserId = user.Id;

            await _teacherService.AddTeacherAsync(teacher);

            TempData["Success"] = "Teacher created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null)
                return NotFound();

            var viewModel = _mapper.Map<TeacherViewModel>(teacher);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TeacherViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var teacher = _mapper.Map<Teacher>(model);
            await _teacherService.UpdateTeacherAsync(teacher);

            TempData["Success"] = "Teacher updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _teacherService.DeleteTeacherAsync(id);
            TempData["Success"] = "Teacher deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
