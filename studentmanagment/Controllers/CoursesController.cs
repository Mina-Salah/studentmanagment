using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Core.Entities.Course_file;
using StudentManagement.Services.Interfaces;
using StudentManagement.Web.Helpers;
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

            course.CourseTeachers = model.SelectedTeacherIds.Select(tid => new CourseTeacher
            {
                TeacherId = tid,
                Course = course
            }).ToList();

            await _courseService.AddCourseAsync(course);

            // ✅ إنشاء مجلد الكورس
            var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");
            FileHelper.CreateCourseFolder(wwwRootPath, course.Title);
            return RedirectToAction(nameof(Index));
        }


        // GET: /Courses/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound();

            var viewModel = _mapper.Map<CourseViewModel>(course);

            // ⬅️ حدد المدرسين المرتبطين بالكورس الحالي
            viewModel.SelectedTeacherIds = course.CourseTeachers?
                .Select(ct => ct.TeacherId)
                .ToList() ?? new List<int>();

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

            var courseInDb = await _courseService.GetCourseByIdAsync(id);
            if (courseInDb == null)
                return NotFound();

            string oldTitle = courseInDb.Title;

            // تحديث البيانات
            courseInDb.Title = model.Title;
            courseInDb.Description = model.Description;
            courseInDb.StartDate = model.StartDate;
            courseInDb.EndDate = model.EndDate;
            courseInDb.CategoryId = model.CategoryId;

            // تحديث المدرسين
            courseInDb.CourseTeachers.Clear();
            courseInDb.CourseTeachers = model.SelectedTeacherIds.Select(tid => new CourseTeacher
            {
                CourseId = courseInDb.Id,
                TeacherId = tid
            }).ToList();

            await _courseService.UpdateCourseAsync(courseInDb);

            // ✅ تحديث المجلد إذا تغير الاسم
            var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");
            var oldFolder = Path.Combine(wwwRootPath, oldTitle);
            var newFolder = Path.Combine(wwwRootPath, model.Title);

            if (oldTitle != model.Title && Directory.Exists(oldFolder))
            {
                Directory.Move(oldFolder, newFolder);
            }
            else if (!Directory.Exists(newFolder))
            {
                FileHelper.CreateCourseFolder(wwwRootPath, model.Title);
            }

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
            var list = courses.ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                list = list.Where(c =>
                    (!string.IsNullOrEmpty(c.Title) && c.Title.ToLower().Contains(search)) ||
                    c.CourseTeachers.Any(ct => ct.Teacher.Name.ToLower().Contains(search))
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
