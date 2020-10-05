using HomeTask4.Core.Entities;
namespace HomeTask4.Infrastructure.Data.Repositories
{
    public class RecipeStepRepository : Repository<RecipeStep>
    {
        public RecipeStepRepository(Task4DBContext context) : base(context) { }
    }
}