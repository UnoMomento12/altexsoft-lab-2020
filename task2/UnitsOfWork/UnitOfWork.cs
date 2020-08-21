using task2.Repositories;
using task2.Models;
using task2.DataManager;
using System.Collections.Generic;

namespace task2.UnitsOfWork
{
    class UnitOfWork 
    {
        private IDataManager _dataManager;
        public IRepository<Recipe> Recipes { get; }
        public IRepository<Ingredient> Ingredients { get;  }
        public IRepository<Category> Categories { get; }
        public UnitOfWork()
        {
            _dataManager = new JsonDataManager();
            
            Ingredients = new IngredientRepository(_dataManager);
            Recipes = new RecipeRepository(_dataManager);
            Categories = new CategoryRepository(_dataManager);
            

            foreach (var recipe in Recipes.GetItems())
            {
                recipe.Ingredients = RestoreIngredients(recipe);
            }
            foreach (var cat in Categories.GetItems())
            {
                cat.Recipes = RestoreRecipesInCategory(cat);
                cat.Parent = RestoreParent(cat);
            }
        }
        
        public void Save()
        {
            Ingredients.Save();
            Recipes.Save();
            Categories.Save();
        }
        
        private List<IngredientDetail> RestoreIngredients(Recipe recipe)
        {
            List<IngredientDetail> result = new List<IngredientDetail>();
            foreach(var item in recipe.IngIdAndAmount)
            {
                result.Add(new IngredientDetail(Ingredients.Get(item.Key) ,item.Value));
            }
            return result;
        }
        private Category RestoreParent(Category category)
        {
            return Categories.SingleOrDefault(x=> x.ID == category.ParentID);
        }
        private List<Recipe> RestoreRecipesInCategory(Category category)
        {
            List<Recipe> result = new List<Recipe>();
            foreach (var val in category.RecipeIds)
            {
                result.Add(Recipes.Get(val));
            }
            return result;
        }
    }
}
