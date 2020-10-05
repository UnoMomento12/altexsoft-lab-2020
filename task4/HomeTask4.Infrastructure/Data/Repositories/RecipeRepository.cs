using HomeTask4.Core.Entities;
namespace HomeTask4.Infrastructure.Data.Repositories
{
    public class RecipeRepository : Repository<Recipe>
    {
        public RecipeRepository(Task4DBContext context) : base(context) { }
    }
}