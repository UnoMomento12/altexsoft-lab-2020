using HomeTask4.Core.Entities;
namespace HomeTask4.Infrastructure.Data.Repositories
{
    public class CategoryRepository : Repository<Category>
    {
        public CategoryRepository(Task4DBContext context) : base(context) { }
    }
}
