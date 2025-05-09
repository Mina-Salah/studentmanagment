using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces;
using StudentManagement.Web.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using StudentManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace StudentManagement.Web.Controllers
{
    [Authorize]

    public class SubjectsController : Controller
    {
        private readonly ISubjectService _subjectService;
        private readonly IMapper _mapper;

        public SubjectsController(ISubjectService subjectService, IMapper mapper)
        {
            _subjectService = subjectService;
            _mapper = mapper;
        }

        // GET: Subjects
        public async Task<IActionResult> Index()
        {
            var subjects = await _subjectService.GetAllSubjectsAsync();
            var viewModels = _mapper.Map<IEnumerable<SubjectCheckboxItem>>(subjects);
            return View(viewModels);
        }

        // GET: Subjects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subjects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubjectCheckboxItem model)
        {
            if (ModelState.IsValid)
            {
                // Map SubjectViewModel to Subject entity
                var subject = _mapper.Map<Subject>(model);
                // Save the new subject
                await _subjectService.AddSubjectAsync(subject);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Subjects/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if (subject == null)
                return NotFound();
            var viewModel = _mapper.Map<SubjectCheckboxItem>(subject);
            return View(viewModel);
        }

        // POST: Subjects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubjectCheckboxItem model)
        {
            if (id != model.Id)
                return BadRequest();
            if (!ModelState.IsValid)
                return View(model);
            var subject = _mapper.Map<Subject>(model);
            await _subjectService.UpdateSubjectAsync(subject);
            return RedirectToAction(nameof(Index));
        }

        // GET: Subjects/GetSubjectDetails/5
        [HttpGet]
        public async Task<IActionResult> GetSubjectDetails(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if (subject == null)
                return NotFound();
            var viewModel = _mapper.Map<SubjectCheckboxItem>(subject);
            return Json(viewModel);
        }

        // POST: Subjects/DeleteSubject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var result = new { success = false, message = "" };

            try
            {
                await _subjectService.DeleteSubjectAsync(id);
                result = new { success = true, message = "تم حذف المادة بنجاح" };
            }
            catch
            {
                result = new { success = false, message = "حدث خطأ أثناء محاولة حذف المادة" };
            }

            return Json(result);
        }

		[HttpPost]
		[ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _subjectService.DeleteSubjectAsync(id);
                TempData["SuccessMessage"] = "تم حذف المادة بنجاح";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء محاولة حذف المادة";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}