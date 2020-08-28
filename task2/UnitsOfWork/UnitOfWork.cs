using task2.Repositories;
using task2.Models;
using task2.DataManager;
using task2.EntityList;
namespace task2.UnitsOfWork
{
    class UnitOfWork : IUnitOfWork
    {
        private Entities _entities; //like context
        public IRepository<Recipe> Recipes { get; }
        public IRepository<Ingredient> Ingredients { get; }
        public IRepository<Category> Categories { get; }
        public UnitOfWork()
        {
            _entities = new Entities();
            Ingredients = new IngredientRepository(_entities);
            Recipes = new RecipeRepository(_entities);
            Categories = new CategoryRepository(_entities);
        }
        
        public void Save()
        {
            _entities.Save();
        }
    }
}
