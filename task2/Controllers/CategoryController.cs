using System;
using task2.Models;
using task2.UnitsOfWork;
namespace task2.Controllers
{
    class CategoryController : BaseController
    {
        public CategoryController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            RestoreReferencesInCategories();
        }

        private void RestoreReferencesInCategories()
        {
            foreach (var category in WorkingUnit.Categories.GetItems())
            {
                category.Parent = RestoreParent(category);
            }
        }
        private Category RestoreParent(Category category)
        {
            return WorkingUnit.Categories.SingleOrDefault(x => x.Id == category.ParentId);
        }
        
        public bool TryCreateCategory(Category category)
        {
            CreateCategory(category);
            bool result = WorkingUnit.Categories.Get(category.Id) != null ? true : false;
            return result;
        }
        public bool TryCreateCategory(string categoryName, string parentId = null)
        {
            return TryCreateCategory(new Category { Name = categoryName, ParentId = parentId });
        }

        public void CreateCategory(Category category)
        {
            var item = WorkingUnit.Categories.SingleOrDefault(x => string.Equals(x.Name, category.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == category.ParentId);
            if (item != null)
            {
                Console.WriteLine($"Category {item.Name} already exists!");
                return;
            } 
            if (string.IsNullOrEmpty(category.Id))
                category.Id = Guid.NewGuid().ToString();
            if (category.Parent == null)
                category.Parent = WorkingUnit.Categories.SingleOrDefault(x => x.Id == category.ParentId);
            WorkingUnit.Categories.Add(category);
            WorkingUnit.Save();
        }
    }
}
