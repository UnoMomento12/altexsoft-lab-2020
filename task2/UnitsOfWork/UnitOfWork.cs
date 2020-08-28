using task2.Repositories;
using task2.Models;
using task2.DataManager;
using System.Collections.Generic;

namespace task2.UnitsOfWork
{
    class UnitOfWork : IUnitOfWork
    {
        private IDataManager _dataManager;
        public IRepository<Recipe> Recipes { get; }
        public IRepository<Ingredient> Ingredients { get; }
        public IRepository<Category> Categories { get; }
        public UnitOfWork(IDataManager dataManager)
        {
            _dataManager = dataManager;
            Ingredients = new IngredientRepository(_dataManager);
            Recipes = new RecipeRepository(_dataManager);
            Categories = new CategoryRepository(_dataManager);
        }
        
        public void Save()
        {
            Ingredients.Save();
            Recipes.Save();
            Categories.Save();
        }

        
    }
}
