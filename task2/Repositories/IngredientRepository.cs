using task2.Models;
using task2.EntityList;
namespace task2.Repositories
{
    class IngredientRepository :Repository<Ingredient>
    {
        public IngredientRepository(Entities entities) : base(entities)
        {
            ItemsInRepository = entities.IngredientList;
        }

        
    }
}
