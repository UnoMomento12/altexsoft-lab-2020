using HomeTask4.Core.Entities;
using HomeTask4.SharedKernel.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HomeTask4.Core.Controllers
{
    public class CategoryController : BaseController
    {
        public CategoryController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        
        public async Task<bool> TryCreateCategoryAsync(Category category)
        {
            await CreateCategory(category);
            bool result = await _unitOfWork.Repository.GetByIdAsync<Category>(category.Id) != null ? true : false;
            return result;
        }
        public async Task<bool> TryCreateCategory(string categoryName, int? parentId)
        {
            if (String.IsNullOrEmpty(categoryName))
            {
                return false;
            }
            return await TryCreateCategoryAsync(new Category { Name = categoryName, ParentId = parentId });
        }

        public async Task CreateCategory(Category category)
        {
            var item = (await _unitOfWork.Repository.ListAsync<Category>()).SingleOrDefault(x => string.Equals(x.Name, category.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == category.ParentId);
            if (item != null)
            {
                Console.WriteLine($"Category {item.Name} already exists!");
                return;
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
