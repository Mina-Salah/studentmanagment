using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces;
using StudentManagement.Services.Interfaces;
using StudentManagement.Web.ViewModels;

namespace StudentManagement.Web.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentsController(IStudentService studentService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _studentService = studentService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _studentService.GetAllStudentsAsync();

            var viewModels = students.Select(s =>
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

        [HttpPost]
        public async Task<IActionResult> Create(StudentFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var student = _mapper.Map<Student>(model);

            var selectedSubjectIds = model.Subjects
                .Where(s => s.IsSelected)
                .Select(s => s.Id)
                .ToList();

            await _studentService.CreateStudentAsync(student, selectedSubjectIds);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound();

            var viewModel = _mapper.Map<StudentFormViewModel>(student);

            var allSubjects = await _unitOfWork.Subjects.GetAllAsync();
            viewModel.Subjects = allSubjects.Select(s => new SubjectCheckboxItem
            {
                Id = s.Id,
                Name = s.Name,
                IsSelected = student.StudentSubjects.Any(ss => ss.SubjectId == s.Id)
            }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(StudentFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var student = _mapper.Map<Student>(model);

            var selectedSubjectIds = model.Subjects
                .Where(s => s.IsSelected)
                .Select(s => s.Id)
                .ToList();

            await _studentService.UpdateStudentAsync(model.Id, student, selectedSubjectIds);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _studentService.DeleteStudentAsync(id);
                TempData["SuccessMessage"] = "تم حذف الطالب بنجاح";
            }
            catch
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء محاولة حذف الطالب";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
