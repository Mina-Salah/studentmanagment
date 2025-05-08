using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Core.Entities;
using StudentManagement.Services.Interfaces;
using StudentManagement.Web.ViewModels;
using StudentManagement.Core.Interfaces;

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

            var viewModels = students.Select(s => new StudentFormViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address,
                DateOfBirth = s.DateOfBirth,
                Subjects = s.StudentSubjects.Select(ss => ss.Subject.Name).ToList()
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

            var allSubjects = await _unitOfWork.Subjects.GetAllAsync();

            var viewModel = new StudentFormViewModel
            {
                Id = student.Id,
                Name = student.Name,
                DateOfBirth = student.DateOfBirth,
                Address = student.Address,
                Subjects = allSubjects.Select(s => new SubjectCheckboxItem
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsSelected = student.StudentSubjects.Any(ss => ss.SubjectId == s.Id)
                }).ToList()
            };

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

        public async Task<IActionResult> Delete(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound();

            var viewModel = _mapper.Map<StudentFormViewModel>(student);
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _studentService.DeleteStudentAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
