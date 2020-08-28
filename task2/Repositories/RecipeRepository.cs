using task2.EntityList;
using task2.Models;
namespace task2.Repositories
{
    class RecipeRepository : Repository<Recipe>
    {
        public RecipeRepository(Entities entities) : base(entities) 
        {
            ItemsInRepository = entities.RecipeList;
        }
        
        public override void Add(Recipe recipe)
        {
            foreach(var a in recipe.Ingredients)
            {
                recipe.IngIdAndAmount.Add(a.Ingredient.Id, a.Amount );
            }
            ItemsInRepository.Add(recipe);
        }
    }
}
