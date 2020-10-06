namespace HomeTask4.Infrastructure.Data.Repositories
{
    public class RecipeStepRepository : Repository
    {
        public RecipeStepRepository(Task4DBContext context) : base(context) { }
    }
}