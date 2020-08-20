using task2.Repositories;
using task2.Models;
using task2.DataManager;
using System.Collections.Generic;
using System;

namespace task2.UnitsOfWork
{
    class UnitOfWork 
    {
        private IDataManager _dataManager;
        private Entities _entities;
        public IRepository<Recipe> Recipes { get; set; }
        public IRepository<Ingredient> Ingredients { get; set; }
        public IRepository<Category> Categories { get; set; }
        


        public UnitOfWork() : this(new Entities()) { }
        public UnitOfWork(Entities entities)
        {
            _dataManager = new JsonDataManager();
            _entities = entities;
            Ingredients = new IngredientRepository(_dataManager,_entities);
            Recipes = new RecipeRepository(_dataManager,_entities);
            Categories = new CategoryRepository(_dataManager,_entities);
            

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
