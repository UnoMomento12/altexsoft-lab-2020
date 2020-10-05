using HomeTask4.Core.Entities;
using HomeTask4.SharedKernel.Interfaces;
using System;
using System.Linq;
namespace HomeTask4.Core.Controllers
{
    public class CategoryController : BaseController
    {
        public CategoryController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            RestoreReferencesInCategories();
        }

        private async void RestoreReferencesInCategories()
        {
            foreach (var category in (await _unitOfWork.Categories.GetAllAsync()))
            {
                category.Parent = RestoreParent(category);
            }
        }
        private Category RestoreParent(Category category)
        {
            return _unitOfWork.Categories.GetAll().SingleOrDefault(x => x.Id == category.ParentId);
        }
        
        public bool TryCreateCategory(Category category)
        {
            CreateCategory(category);
            bool result = _unitOfWork.Categories.GetById(category.Id) != null ? true : false;
            return result;
        }
        public bool TryCreateCategory(string categoryName, string parentId = null)
        {
            return TryCreateCategory(new Category { Name = categoryName, ParentId = parentId });
        }

        public void CreateCategory(Category category)
        {
            var item = _unitOfWork.Categories.GetAll().SingleOrDefault(x => string.Equals(x.Name, category.Name, StringComparison.OrdinalIgnoreCase) && x.ParentId == category.ParentId);
            if (item != null)
            {
                Console.WriteLine($"Category {item.Name} already exists!");
                return;
            } 
            if (string.IsNullOrEmpty(category.Id))
                category.Id = Guid.NewGuid().ToString();
            if (category.Parent == null)
                category.Parent = _unitOfWork.Categories.GetAll().SingleOrDefault(x => x.Id == category.ParentId);
            _unitOfWork.Categories.Add(category);
            _unitOfWork.Save();
        }
    }
}
