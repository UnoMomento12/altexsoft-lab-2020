using System;
using System.Linq;
using task2.Models;
using task2.UnitsOfWork;
namespace task2.Controllers
{
    class CategoryController : BaseController
    {
        public CategoryController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public bool TryCreateCategory(Category category)
        {
            CreateCategory(category);
            bool result = _unitOfWork.Categories.Get(category.Id) != null ? true : false;
            return result;
        }
        public bool TryCreateCategory(string categoryName, string parentId = null)
        {
            return TryCreateCategory(new Category { Name = categoryName, ParentId = parentId });
        }

        public void CreateCategory(Category category)
        {
            var item = _unitOfWork.Categories.SingleOrDefault(x => string.Equals(x.Name, category.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == category.ParentId);
            if (item != null)
            {
                category = item;
                return;
            }
            if (string.IsNullOrEmpty(category.Id))
                category.Id = Guid.NewGuid().ToString();
            if (category.Parent == null)
                category.Parent = _unitOfWork.Categories.SingleOrDefault(x => x.Id == category.ParentId);
            _unitOfWork.Categories.Add(category);
            _unitOfWork.Save();
        }


        public void AddRecipeToCategory(Category category, Recipe recipe)
        {
            var recip = category.Recipes.SingleOrDefault(x => x.Id == recipe.Id);
            if(recip != null)
            {
                Console.WriteLine("Recipe is already in category");
                return;
            }
            category.Recipes.Add(recipe);
            category.RecipeIds.Add(recipe.Id);
            _unitOfWork.Save();
        }
    }
}
