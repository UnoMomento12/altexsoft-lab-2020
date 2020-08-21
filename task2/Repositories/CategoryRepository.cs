using System.Collections.Generic;
using task2.DataManager;
using task2.Models;

namespace task2.Repositories
{
    class CategoryRepository : Repository<Category>
    {
        public CategoryRepository(IDataManager dataManager ) : base(dataManager)
        {
           
        }
        
        public override void Add(Category category)
        {
            foreach (var a in category.Recipes)
            {
                category.RecipeIds.Add(a.ID);
            }
            category.ParentID = category.Parent?.ID;
            _items.Add(category);
        }
    }
}
