namespace HomeTask4.Infrastructure.Data.Repositories
{
    public class IngredientRepository : Repository
    {
        public IngredientRepository(Task4DBContext context) : base(context) { }
    }
}