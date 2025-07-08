using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces;
using StudentManagement.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _unitOfWork.Categories.GetAllAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _unitOfWork.Categories.GetByIdAsync(id);
        }

        public async Task AddCategoryAsync(Category category)
        {
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            await _unitOfWork.Categories.SoftDeleteAsync(id);
            await _unitOfWork.CompleteAsync();

        }

        public async Task PermanentDeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdIncludingDeletedAsync(id);
            if (category != null)
            {
                _unitOfWork.Categories.Delete(category);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task RestoreCategoryAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdIncludingDeletedAsync(id);
            if (category != null && category.IsDeleted)
            {
                category.IsDeleted = false;
                _unitOfWork.Categories.Update(category);
                await _unitOfWork.CompleteAsync();
            }
        }

    }
}
