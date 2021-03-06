﻿using Castle.Core.Internal;
using HomeTask4.Core.Entities;
using HomeTask4.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeTask4.Core.Exceptions;
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
            catch( EmptyFieldException emptyFieldException)
            {
                Logger.LogError(emptyFieldException.Message);
                throw;
            } catch ( EntryAlreadyExistsException entryAlreadyExistsException)
            {
                Logger.LogError(entryAlreadyExistsException.Message);
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
            if (category.Name.IsNullOrEmpty()) throw new EmptyFieldException("Category name is null or empty!");
            var item = await UnitOfWork.Repository.FirstOrDefaultAsync<Category>(x => x.Name.ToLower() == category.Name.ToLower() && x.ParentId == category.ParentId);
            if (item != null)
            {
                throw new EntryAlreadyExistsException($"This {category.Name} category already exists !");
            }
            await UnitOfWork.Repository.AddAsync<Category>(category);
        }
        public Task<List<Category>> GetAllCategoriesAsync() {
            return UnitOfWork.Repository.ListAsync<Category>();
        }

        public Task<List<Category>> GetCategoriesByParentId(int? parentId)
        {
            return UnitOfWork.Repository.WhereAsync<Category>(x => x.ParentId == parentId);
        }
        public Task<Category> GetCategoryByIdAsync(int id)
        {
            return UnitOfWork.Repository.GetByIdAsync<Category>(id);
        }
        public async Task DeleteCategoryByIdAsync(int id)
        {
            Category toDelete = await UnitOfWork.Repository.GetByIdAsync<Category>(id);
            if (toDelete == null)
            {
                throw new EntryNotFoundException($"The category doesn't exist in database.");
            }
            await UnitOfWork.Repository.DeleteAsync<Category>(toDelete);
        }
        public async Task UpdateCategoryAsync(Category toUpdate)
        {
            var retrieved = await UnitOfWork.Repository.GetByIdAsync<Category>(toUpdate.Id);
            if (retrieved == null)
            {
                throw new EntryNotFoundException($"The {toUpdate.Name} doesn't exist in database.");
            }
            retrieved.Name = toUpdate.Name;
            await UnitOfWork.Repository.UpdateAsync(retrieved);
        }
    }
}
