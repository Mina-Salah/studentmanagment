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
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentsController(
            IStudentService studentService,
            IUserService userService,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _studentService = studentService;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // عرض الطلاب غير المحذوفين
        public async Task<IActionResult> Index()
        {
            var students = await _studentService.GetAllStudentsAsync();

            var viewModels = students.Select(s =>
            {
                var vm = _mapper.Map<StudentFormViewModel>(s);
                vm.Email = s.User?.Email; 
                vm.Subjects = s.StudentSubjects.Select(ss => new SubjectCheckboxItem
                {
                    Id = ss.SubjectId,
                    Name = ss.Subject.Name,
                    IsSelected = true
                }).ToList();
                return vm;
            }).ToList();

            return View(viewModels);
        }

        // عرض فورم إنشاء طالب
        public async Task<IActionResult> Create()
        {
            var subjects = await _unitOfWork.Subjects.GetAllAsync();

            var viewModel = new StudentFormViewModel
            {
                Subjects = subjects.Select(s => new SubjectCheckboxItem
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsSelected = false
                }).ToList()
            };

            return View(viewModel);
        }

        // إنشاء طالب جديد
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentFormViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Password))
            {
                if (string.IsNullOrWhiteSpace(model.Password))
                    ModelState.AddModelError("Password", "كلمة المرور مطلوبة.");

                await PopulateSubjectsAsync(model);
                return View(model);
            }

            if (await _userService.IsEmailTakenAsync(model.Email))
            {
                ModelState.AddModelError("Email", "هذا الإيميل مستخدم بالفعل.");
                await PopulateSubjectsAsync(model);
                return View(model);
            }

            var student = _mapper.Map<Student>(model);
            var selectedSubjectIds = model.Subjects?
                .Where(s => s.IsSelected)
                .Select(s => s.Id)
                .ToList() ?? new();

            await _studentService.CreateStudentAsync(student, selectedSubjectIds, model.Email, model.Password);

            TempData["SuccessMessage"] = $"تم إنشاء الطالب بنجاح. البريد: {model.Email}، كلمة السر: {model.Password}";
            return RedirectToAction(nameof(Index));
        }

        // تعبئة قائمة المواد
        private async Task PopulateSubjectsAsync(StudentFormViewModel model)
        {
            var allSubjects = await _unitOfWork.Subjects.GetAllAsync();

            model.Subjects = allSubjects.Select(s => new SubjectCheckboxItem
            {
                Id = s.Id,
                Name = s.Name,
                IsSelected = model.Subjects?.Any(ms => ms.Id == s.Id && ms.IsSelected) == true
            }).ToList();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null) return NotFound();

            var viewModel = _mapper.Map<StudentFormViewModel>(student);

            // ✅ جلب كل المواد
            var allSubjects = await _unitOfWork.Subjects.GetAllAsync();

            // ✅ المواد المختارة فعلاً
            var selectedSubjectIds = student.StudentSubjects.Select(ss => ss.SubjectId).ToList();

            // ✅ إعداد الـ Subjects في الـ ViewModel
            viewModel.Subjects = allSubjects.Select(subject => new SubjectCheckboxItem
            {
                Id = subject.Id,
                Name = subject.Name,
                IsSelected = selectedSubjectIds.Contains(subject.Id)
            }).ToList();

            // جلب الإيميل من المستخدم
            var user = (await _unitOfWork.Users.GetAllAsync())
                .FirstOrDefault(u => u.FullName == student.Name && u.Role == "Student");

            if (user != null)
                viewModel.Email = user.Email;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateSubjectsAsync(model);
                return View(model);
            }

            var student = _mapper.Map<Student>(model);
            var selectedSubjectIds = model.Subjects?
                .Where(s => s.IsSelected)
                .Select(s => s.Id)
                .ToList() ?? new();

            // ✅ جلب الطالب الفعلي من قاعدة البيانات للتأكد من وجوده ومعرفة UserId
            var existingStudent = await _studentService.GetStudentByIdAsync(model.Id);
            if (existingStudent == null)
            {
                return NotFound();
            }

            // ✅ جلب المستخدم المرتبط باستخدام UserId وليس FullName
            User? user = null;
            if (existingStudent.UserId.HasValue)
            {
                user = await _unitOfWork.Users.GetByIdAsync(existingStudent.UserId.Value);
            }

            // ✅ التحقق من التكرار فقط إذا تم تغيير الإيميل
            if (user != null && user.Email.ToLower() != model.Email.ToLower() &&
                await _userService.IsEmailTakenAsync(model.Email))
            {
                ModelState.AddModelError("Email", "هذا الإيميل مستخدم بالفعل.");
                await PopulateSubjectsAsync(model);
                return View(model);
            }

            // ✅ تحديث بيانات الطالب
            await _studentService.UpdateStudentAsync(model.Id, student, selectedSubjectIds);

            if (user != null)
            {
                user.Email = model.Email;
                user.FullName = model.Name;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();
            }

            TempData["SuccessMessage"] = "تم تحديث بيانات الطالب بنجاح.";
            return RedirectToAction(nameof(Index));
        }


        // حذف الطالب (Soft Delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _studentService.DeleteStudentAsync(id);
                TempData["SuccessMessage"] = "تم حذف الطالب بنجاح.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"حدث خطأ أثناء الحذف: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // عرض الطلاب المحذوفين
        public async Task<IActionResult> Deleted()
        {
            var deletedStudents = await _studentService.GetDeletedStudentsAsync();

            var viewModels = deletedStudents.Select(s =>
            {
                var vm = _mapper.Map<StudentFormViewModel>(s);
                vm.Subjects = s.StudentSubjects.Select(ss => new SubjectCheckboxItem
                {
                    Id = ss.SubjectId,
                    Name = ss.Subject.Name,
                    IsSelected = true
                }).ToList();
                return vm;
            }).ToList();

            return View(viewModels);
        }

        // استرجاع طالب
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                await _studentService.RestoreStudentAsync(id);
                TempData["SuccessMessage"] = "تم استرجاع الطالب بنجاح.";
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
                await _studentService.DeleteStudentPermanentlyAsync(id);
                return RedirectToAction("Deleted");
            }
            catch (Exception ex)
            {
                // ممكن تسجل الخطأ أو تعرضه للمستخدم
                return BadRequest(ex.Message);
            }
        }

    }
}
