using task2.DataManager;
using task2.Models;
namespace task2.Repositories
{
    class RecipeRepository : Repository<Recipe>
    {
        public RecipeRepository(IDataManager dataManager) : base(dataManager) 
        {
            
        }
        
        public override void Add(Recipe recipe)
        {
            foreach(var a in recipe.Ingredients)
            {
                recipe.IngIdAndAmount.Add(a.Ingredient.ID, a.Amount );
            }
            _items.Add(recipe);
        }
    }
}
