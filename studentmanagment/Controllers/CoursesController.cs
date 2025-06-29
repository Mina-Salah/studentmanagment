using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Core.Entities.Course_file;
using StudentManagement.Services.Interfaces;
using StudentManagement.Web.ViewModels;

namespace StudentManagement.Web.Controllers
{
    [Authorize(Roles = "Admin")]

    public class CoursesController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ITeacherService _teacherService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CoursesController(
            ICourseService courseService,
            ITeacherService teacherService,
            ICategoryService categoryService,
            IMapper mapper)
        {
            _courseService = courseService;
            _teacherService = teacherService;
            _categoryService = categoryService;
            _mapper = mapper;
        }

        // GET: /Courses
        public async Task<IActionResult> Index(string? search, int? categoryId, string? sortOrder)
        {
            var courses = await _courseService.GetAllCoursesAsync();
            var filtered = ApplyFilters(courses, search, categoryId, sortOrder);

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentCategory = categoryId;

            var courseVMs = _mapper.Map<List<CourseViewModel>>(filtered);
            return View(courseVMs);
        }

        // GET: /Courses/FilterCourses (AJAX)
        [HttpGet]
        public async Task<IActionResult> FilterCourses(string? search, int? categoryId, string? sortOrder)
        {
            var courses = await _courseService.GetAllCoursesAsync();
            var filtered = ApplyFilters(courses, search, categoryId, sortOrder);
            var viewModels = _mapper.Map<List<CourseViewModel>>(filtered);

            return PartialView("_CourseTablePartial", viewModels);
        }

        // GET: /Courses/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new CourseViewModel
            {
                Teachers = await _teacherService.GetAllTeachersAsync(),
                Categories = await _categoryService.GetAllCategoriesAsync()
            };

            return View(viewModel);
        }

        // POST: /Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Teachers = await _teacherService.GetAllTeachersAsync();
                model.Categories = await _categoryService.GetAllCategoriesAsync();
                return View(model);
            }

            var course = _mapper.Map<Course>(model);
            await _courseService.AddCourseAsync(course);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Courses/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound();

            var viewModel = _mapper.Map<CourseViewModel>(course);
            viewModel.Teachers = await _teacherService.GetAllTeachersAsync();
            viewModel.Categories = await _categoryService.GetAllCategoriesAsync();

            return View(viewModel);
        }

        // POST: /Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                model.Teachers = await _teacherService.GetAllTeachersAsync();
                model.Categories = await _categoryService.GetAllCategoriesAsync();
                return View(model);
            }

            var course = _mapper.Map<Course>(model);
            await _courseService.UpdateCourseAsync(course);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Courses/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _courseService.DeleteCourseAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // 🔧 Private helper for filtering logic
        private IEnumerable<Course> ApplyFilters(IEnumerable<Course> courses, string? search, int? categoryId, string? sortOrder)
        {
            var list = courses.ToList(); // تأكد إنها In-Memory

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                list = list.Where(c =>
                    (!string.IsNullOrEmpty(c.Title) && c.Title.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(c.Teacher?.Name) && c.Teacher.Name.ToLower().Contains(search))
                ).ToList();
            }

            if (categoryId.HasValue)
                list = list.Where(c => c.CategoryId == categoryId.Value).ToList();

            list = sortOrder switch
            {
                "start_asc" => list.OrderBy(c => c.StartDate).ToList(),
                "start_desc" => list.OrderByDescending(c => c.StartDate).ToList(),
                _ => list.OrderBy(c => c.Title).ToList()
            };

            return list;
        }

    }
}
