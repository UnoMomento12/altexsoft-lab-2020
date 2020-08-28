using task2.DataManager;
using task2.Models;

namespace task2.Repositories
{
    class CategoryRepository : Repository<Category>
    {
        public CategoryRepository(IDataManager dataManager ) : base(dataManager)
        {
           
        }
        
    }
}
