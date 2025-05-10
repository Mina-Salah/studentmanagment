using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces;
using StudentManagement.Services.Interfaces;
using StudentManagement.Web.ViewModels;

namespace StudentManagement.Web.Controllers
{
    [Authorize] // Restricts access to authenticated users only
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        // Constructor injecting 
        public StudentsController(IStudentService studentService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _studentService = studentService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: /Students
        public async Task<IActionResult> Index()
        {
            // Retrieve all students
            var students = await _studentService.GetAllStudentsAsync();

            // Map student entities to view models and include their selected subjects
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

        // GET: /Students/Create
        public async Task<IActionResult> Create()
        {
            // Get all subjects ....  checkboxes
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

        // POST: /Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Map the view model to a Student entity
            var student = _mapper.Map<Student>(model);

            // Get selected subject IDs or empty list if none selected
            var selectedSubjectIds = model.Subjects?
                .Where(s => s.IsSelected)
                .Select(s => s.Id)
                .ToList() ?? new List<int>();

            // Create the student with selected subjects
            await _studentService.CreateStudentAsync(student, selectedSubjectIds);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Students/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            // Retrieve the student to edit
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound();

            // Map entity to view model
            var viewModel = _mapper.Map<StudentFormViewModel>(student);

            // Load all subjects and mark those already assigned to student
            var allSubjects = await _unitOfWork.Subjects.GetAllAsync();
            viewModel.Subjects = allSubjects.Select(s => new SubjectCheckboxItem
            {
                Id = s.Id,
                Name = s.Name,
                IsSelected = student.StudentSubjects.Any(ss => ss.SubjectId == s.Id)
            }).ToList();

            return View(viewModel);
        }

        // POST: /Students/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload subjects if validation fails
                var allSubjects = await _unitOfWork.Subjects.GetAllAsync();
                model.Subjects = allSubjects.Select(s => new SubjectCheckboxItem
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsSelected = model.Subjects?.Any(ms => ms.Id == s.Id && ms.IsSelected) == true
                }).ToList();

                return View(model);
            }

            // Map the view model to a Student entity
            var student = _mapper.Map<Student>(model);

            // Get selected subject IDs
            var selectedSubjectIds = model.Subjects
                .Where(s => s.IsSelected)
                .Select(s => s.Id)
                .ToList();

            try
            {
                // Update the student
                await _studentService.UpdateStudentAsync(model.Id, student, selectedSubjectIds);
                TempData["SuccessMessage"] = "Student updated successfully";
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while updating the student";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Students/Deleted
        public async Task<IActionResult> Deleted()
        {
            // Get soft-deleted students
            var students = await _studentService.GetDeletedStudentsAsync();

            // Map to view models and include their subjects
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

        // POST: /Students/Restore
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                // Restore soft-deleted student
                await _studentService.RestoreStudentAsync(id);
                TempData["SuccessMessage"] = "Student restored successfully";
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while restoring the student";
            }

            return RedirectToAction(nameof(Deleted));
        }

        // POST: /Students/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Soft-delete student
                await _studentService.DeleteStudentAsync(id);
                TempData["SuccessMessage"] = "Student deleted successfully";
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the student";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
