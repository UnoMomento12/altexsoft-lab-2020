using HomeTask4.Core.Entities;
namespace HomeTask4.Infrastructure.Data.Repositories
{
    public class IngredientRepository : Repository<Ingredient>
    {
        public IngredientRepository(Task4DBContext context) : base(context) { }
    }
}