using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Core.Entities;
using StudentManagement.Services.Interfaces;
using System.Threading.Tasks;

namespace StudentManagement.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: /Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }


        // GET: /Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _categoryService.AddCategoryAsync(model);
            TempData["SuccessMessage"] = "Category created successfully";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Categories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: /Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            await _categoryService.UpdateCategoryAsync(model);
            TempData["SuccessMessage"] = "Category updated successfully";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Categories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);
                TempData["SuccessMessage"] = "Category deleted successfully";
            }
            catch
            {
                TempData["ErrorMessage"] = "Error occurred while deleting category.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            await _categoryService.RestoreCategoryAsync(id);
            TempData["SuccessMessage"] = "Category restored successfully";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            await _categoryService.PermanentDeleteCategoryAsync(id);
            TempData["SuccessMessage"] = "Category permanently deleted successfully";
            return RedirectToAction(nameof(Index));
        }

    }
}
