using System;
using System.Linq;
using task2.Models;
using task2.UnitsOfWork;
namespace task2.Controllers
{
    class CategoryController : BaseController
    {
        public CategoryController(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Category CreateAndGetCategory(Category category)
        {
            CreateCategory(category);
            return category;
        }
        public Category CreateAndGetCategory(string categoryName, string parentId = null)
        {
            return CreateAndGetCategory(new Category { Name = categoryName, ParentId = parentId });
        }

        public void CreateCategory(Category category)
        {
            var item = _unitOfWork.Categories.SingleOrDefault(x => String.Equals(x.Name, category.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == category.ParentId);
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

        //public void Add(Category category)
        //{
        //    CreateAndGetCategory(category);
        //}
        //public Category GetCategory(string guid)
        //{
        //    return _unitOfWork.Categories.Get(guid);
        //}

        //public List<Category> GetCategories(string parentId)
        //{
        //    return _unitOfWork.Categories.Where(x => x.ParentID == parentId).ToList();
        //}

        //public void RemoveCategory(string categoryId)
        //{
        //    var item = _unitOfWork.Categories.SingleOrDefault(x => x.ID == categoryId);
        //    if (item == null)
        //    {
        //        Console.WriteLine("Category " + categoryId + " has not been found");
        //        return;
        //    }

        //    _unitOfWork.Categories.Remove(item);
        //    _unitOfWork.Save();
        //}

    }
}
