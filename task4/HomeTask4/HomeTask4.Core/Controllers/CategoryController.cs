using Castle.Core.Internal;
using HomeTask4.Core.Entities;
using HomeTask4.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace HomeTask4.Core.Controllers
{
    public class CategoryController : BaseController
    {
        public CategoryController(IUnitOfWork unitOfWork, ILogger<CategoryController> logger) : base(unitOfWork, logger)
        {
        }

        
        public async Task<bool> TryCreateCategoryAsync(Category category)
        {
            if (category == null) return false;
            try
            {
                await CreateCategoryAsync(category);
            } 
            catch (ArgumentNullException nullException)
            {
                Logger.LogError(nullException.Message);
                throw;
            } 
            catch( ArgumentException argumentException)
            {
                Logger.LogError(argumentException.Message);
                throw;
            } 
            bool result = await UnitOfWork.Repository.GetByIdAsync<Category>(category.Id) != null;
            return result;
        }
        public Task<bool> TryCreateCategoryAsync(string categoryName, int? parentId) 
        {
            return TryCreateCategoryAsync(new Category { Name = categoryName, ParentId = parentId });
        }

        public async Task CreateCategoryAsync(Category category) //everything is thrown here and moves up
        {
            if (category == null) throw new ArgumentNullException($"Category reference is null.");
            if (category.Name.IsNullOrEmpty()) throw new ArgumentException("Category name is null or empty!");
            var item = await UnitOfWork.Repository.FirstOrDefaultAsync<Category>(x => x.Name.ToLower() == category.Name.ToLower() && x.ParentId == category.ParentId);
            if (item != null)
            {
                throw new ArgumentException("This category already exists !");
            }
            await UnitOfWork.Repository.AddAsync<Category>(category);
        }

        private Task<Category> GetParentCategoryAsync(int? parentId)
        {
            return UnitOfWork.Repository.GetByIdAsync<Category>(parentId.GetValueOrDefault());
        }
        public Task<List<Category>> GetAllCategoriesAsync() {
            return UnitOfWork.Repository.ListAsync<Category>();
        }

        public Task<List<Category>> GetCategoriesByParentId(int? parentId)
        {
            return UnitOfWork.Repository.WhereAsync<Category>(x => x.ParentId == parentId);
        }
        public Task<Category> GetCategoryById(int id)
        {
            return UnitOfWork.Repository.GetByIdAsync<Category>(id);
        }
        public async Task DeleteCategoryByIdAsync(int id)
        {
            Category toDelete = await UnitOfWork.Repository.GetByIdAsync<Category>(id);
            await UnitOfWork.Repository.DeleteAsync<Category>(toDelete);
        }
        public async Task UpdateCategoryAsync(Category toUpdate)
        {
            var retrieved = await UnitOfWork.Repository.GetByIdAsync<Category>(toUpdate.Id);
            retrieved.Name = toUpdate.Name;
            await UnitOfWork.Repository.UpdateAsync(retrieved);
        }
    }
}
