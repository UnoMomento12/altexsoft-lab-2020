using HomeTask4.Core.Entities;
namespace HomeTask4.Infrastructure.Data.Repositories
{
    public class IngredientDetailRepository : Repository<IngredientDetail>
    {
        public IngredientDetailRepository(Task4DBContext context) : base(context) { }
    }
}