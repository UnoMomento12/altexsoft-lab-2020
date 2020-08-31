using task2.EntityList;
using task2.Models;

namespace task2.Repositories
{
    class CategoryRepository : Repository<Category>
    {
        public CategoryRepository(Entities entities) : base(entities)
        {
            ItemsInRepository = entities.CategoryList;
        }
    }
}
