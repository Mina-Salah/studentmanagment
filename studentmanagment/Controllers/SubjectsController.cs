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
    [Authorize] // Restricts access to authenticated users only
    public class SubjectsController : Controller
    {
        private readonly ISubjectService _subjectService;
        private readonly IMapper _mapper;

        public SubjectsController(ISubjectService subjectService, IMapper mapper)
        {
            _subjectService = subjectService;
            _mapper = mapper;
        }

        // GET: /Subjects
        // Retrieves all subjects and displays them
        public async Task<IActionResult> Index()
        {
            var subjects = await _subjectService.GetAllSubjectsAsync();
            var viewModels = _mapper.Map<IEnumerable<SubjectCheckboxItem>>(subjects);
            return View(viewModels);
        }

        // GET: /Subjects/Create
        // Returns the form to create a new subject
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Subjects/Create
        // Submits the new subject form
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF protection
        public async Task<IActionResult> Create(SubjectCheckboxItem model)
        {
            if (ModelState.IsValid)
            {
                // Map ViewModel to Subject entity
                var subject = _mapper.Map<Subject>(model);
                // Save subject to the database
                await _subjectService.AddSubjectAsync(subject);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: /Subjects/Edit/{id}
        // Loads subject data for editing
        public async Task<IActionResult> Edit(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if (subject == null)
                return NotFound();

            var viewModel = _mapper.Map<SubjectCheckboxItem>(subject);
            return View(viewModel);
        }

        // POST: /Subjects/Edit/{id}
        // Submits the edited subject data
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

        // GET: /Subjects/GetSubjectDetails/{id}
        // Returns subject data as JSON (used for AJAX requests, etc.)
        [HttpGet]
        public async Task<IActionResult> GetSubjectDetails(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if (subject == null)
                return NotFound();

            var viewModel = _mapper.Map<SubjectCheckboxItem>(subject);
            return Json(viewModel);
        }

        // POST: /Subjects/DeleteSubject
        // Soft deletes a subject and returns a JSON result (for AJAX deletion)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var result = new { success = false, message = "" };

            try
            {
                await _subjectService.DeleteSubjectAsync(id);
                result = new { success = true, message = "Subject deleted successfully" };
            }
            catch (InvalidOperationException ex)
            {
                result = new { success = false, message = ex.Message };
            }
            catch
            {
                result = new { success = false, message = "An error occurred while trying to delete the subject" };
            }

            return Json(result);
        }

        // POST: /Subjects/DeleteConfirmed
        // Deletes a subject with standard form submission (non-AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _subjectService.DeleteSubjectAsync(id);
                TempData["SuccessMessage"] = "Subject deleted successfully";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while trying to delete the subject";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
