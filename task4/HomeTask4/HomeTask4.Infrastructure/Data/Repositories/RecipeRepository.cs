namespace HomeTask4.Infrastructure.Data.Repositories
{
    public class RecipeRepository : Repository
    {
        public RecipeRepository(Task4DBContext context) : base(context) { }
    }
}