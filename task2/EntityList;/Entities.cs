using System.Collections.Generic;
using task2.Models;
using task2.DataManager;
namespace task2.EntityList
{
    class Entities
    {
        private IDataManager _dataManager;
        public readonly List<Ingredient> IngredientList;
        public readonly List<Recipe> RecipeList;
        public readonly List<Category> CategoryList;

        public Entities(IDataManager dataManager)
        {
            _dataManager = dataManager;
            IngredientList = (List<Ingredient>) _dataManager.LoadAndDeserialize<Ingredient>();
            RecipeList = (List<Recipe>)_dataManager.LoadAndDeserialize<Recipe>();
            CategoryList = (List<Category>)_dataManager.LoadAndDeserialize<Category>();
        }

        public void Save()
        {
            _dataManager.SaveToFile<Ingredient>(IngredientList);
            _dataManager.SaveToFile<Recipe>(RecipeList);
            _dataManager.SaveToFile<Category>(CategoryList);
        }
    }
}
