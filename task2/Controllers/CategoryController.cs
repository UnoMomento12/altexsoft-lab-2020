using System;
using System.Collections.Generic;
using System.Linq;
using task2.Models;
using task2.UnitsOfWork;
namespace task2.Controllers
{
    class CategoryController : BaseController
    {
        public CategoryController(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public void Add(Category category)
        {
            CreateCategory(category);
        }
        public Category GetCategory(string guid)
        {
            return _unitOfWork.Categories.Get(guid);
        }

        public List<Category> GetCategories(string parentId)
        {
            return _unitOfWork.Categories.Where(x => x.ParentID == parentId).ToList();
        }
        public Category CreateCategory(Category category)
        {
            var item = _unitOfWork.Categories.SingleOrDefault(x => String.Equals(x.Name, category.Name, StringComparison.OrdinalIgnoreCase) && x.ParentID == category.ParentID);
            if (item != null) return item;
            if (string.IsNullOrEmpty(category.ID))
                category.ID = Guid.NewGuid().ToString();
            if (category.Parent == null)
                category.Parent = _unitOfWork.Categories.SingleOrDefault(x => x.ID == category.ParentID);
            _unitOfWork.Categories.Add(category);
            _unitOfWork.Save();
            return category;
        }

        public Category CreateCategory(string categoryName, string parentId = null)
        {
            return CreateCategory(new Category { Name = categoryName, ParentID = parentId });
        }

        public void RemoveCategory(string categoryId)
        {
            var item = _unitOfWork.Categories.SingleOrDefault(x => x.ID == categoryId);
            if (item == null)
            {
                Console.WriteLine("Category " + categoryId + " has not been found");
                return;
            }

            _unitOfWork.Categories.Remove(item);
            _unitOfWork.Save();
        }

        public void AddRecipeToCategory(Category category, Recipe recipe)
        {
            var recip = category.Recipes.SingleOrDefault(x => x.ID == recipe.ID);
            if(recip != null)
            {
                Console.WriteLine("Recipe is already in category");
                return;
            }
            category.Recipes.Add(recipe);
            category.RecipeIds.Add(recipe.ID);
            _unitOfWork.Save();
        }
    }
}
