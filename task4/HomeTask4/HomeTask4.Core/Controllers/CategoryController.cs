using HomeTask4.Core.Entities;
using HomeTask4.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HomeTask4.Core.Controllers
{
    public class CategoryController : BaseController
    {
        public CategoryController(IUnitOfWork unitOfWork, ILogger<CategoryController> logger) : base(unitOfWork)
        {
            _logger = logger;
        }

        
        public async Task<bool> TryCreateCategoryAsync(Category category)
        {
            try
            {
                await CreateCategory(category);
            } catch (Exception e)
            {
                _logger.LogInformation(e.Message , e);
                return false;
            }
            bool result = await _unitOfWork.Repository.GetByIdAsync<Category>(category.Id) != null ? true : false;
            return result;
        }
        public async Task<bool> TryCreateCategoryAsync(string categoryName, int? parentId)
        {
            if (String.IsNullOrEmpty(categoryName))
            {
                _logger.LogInformation("Failed to create a category, name is empty");
                return false;
            }
            return await TryCreateCategoryAsync(new Category { Name = categoryName, ParentId = parentId });
        }

        public async Task CreateCategory(Category category) 
        {
            var item = (await _unitOfWork.Repository.ListAsync<Category>()).SingleOrDefault(x => string.Equals(x.Name, category.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == category.ParentId);
            if (item != null)
            {
                throw new Exception("This category already exists !");
            }
            if (category.ParentId != null)
            {
                category.Parent = GetParentCategoryAsync(category.ParentId).Result;
            }
            await _unitOfWork.Repository.AddAsync<Category>(category);
        }

        private async Task<Category> GetParentCategoryAsync(int? parentId)
        {
            return (await _unitOfWork.Repository.ListAsync<Category>()).FirstOrDefault(x => x.Id == parentId);
        }
    }
}
