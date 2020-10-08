using Castle.Core.Internal;
using HomeTask4.Core.Entities;
using HomeTask4.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
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
                await CreateCategoryAsync(category);
            } catch (Exception e)
            {
                _logger.LogInformation(e.Message , e);
                return false;
            }
            bool result = await _unitOfWork.Repository.GetByIdAsync<Category>(category.Id) != null;
            return result;
        }
        public async Task<bool> TryCreateCategoryAsync(string categoryName, int? parentId)
        {
            if (String.IsNullOrEmpty(categoryName))
            {
                throw new ArgumentException("Name is null or empty.");
            }
            return await TryCreateCategoryAsync(new Category { Name = categoryName, ParentId = parentId });
        }

        public async Task CreateCategoryAsync(Category category) 
        {
            if (category == null) throw new ArgumentNullException($"Category reference is null.");
            if (category.Name.IsNullOrEmpty()) throw new ArgumentException("Category name is empty!");
            var item = (await _unitOfWork.Repository.ListAsync<Category>()).SingleOrDefault(x => string.Equals(x.Name, category.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == category.ParentId);
            if (item != null)
            {
                throw new ArgumentException("This category already exists !");
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
