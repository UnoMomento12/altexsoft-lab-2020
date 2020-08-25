﻿using task2.Repositories;
using task2.Models;
using task2.DataManager;
using System.Collections.Generic;

namespace task2.UnitsOfWork
{
    class UnitOfWork 
    {
        private IDataManager _dataManager;
        private bool _refDeleted;
        public IRepository<Recipe> Recipes { get; }
        public IRepository<Ingredient> Ingredients { get;  }
        public IRepository<Category> Categories { get; }
        
        public UnitOfWork()
        {
            _dataManager = new JsonDataManager();
            _refDeleted = false;
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
            if (_refDeleted) Save();
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
                result.Add(new IngredientDetail() { Ingredient = Ingredients.Get(item.Key), Amount = item.Value });
            }
            return result;
        }
        private Category RestoreParent(Category category)
        {
            return Categories.SingleOrDefault(x=> x.Id == category.ParentId);
        }
        private List<Recipe> RestoreRecipesInCategory(Category category)
        {
            List<Recipe> result = new List<Recipe>();
            Recipe a;
            List<string> toDeleteInCategory = new List<string>();
            for (int i= 0; i < category.RecipeIds.Count;i++)
            {
                a = Recipes.Get(category.RecipeIds[i]);
                if (a == null)
                {
                    _refDeleted = true;
                    toDeleteInCategory.Add(category.RecipeIds[i]);
                }
                else
                {
                    result.Add(a);
                }
            }
            if (_refDeleted) { DeleteDeadReferences(category, toDeleteInCategory); }
            return result;
        }
        private void DeleteDeadReferences(Category category, List<string> toDeleteInCategory)
        {
            foreach(var val in toDeleteInCategory)
            {
                category.RecipeIds.Remove(val);
            }
        }
    }
}
