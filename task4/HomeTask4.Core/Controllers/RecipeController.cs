using System;
using System.Linq;
using System.Collections.Generic;

namespace HomeTask4.Core.Controllers
{
    public class RecipeController : BaseController
    {
        public RecipeController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            RestoreIngredientsInRecipes();
            RestoreCategoriesInRecipes();
        }
        private void RestoreIngredientsInRecipes()
        {
            foreach (var recipe in _unitOfWork.Recipes.GetAll())
            {
                recipe.Ingredients = RestoreIngredients(recipe);
            }
        }
        private List<IngredientDetail> RestoreIngredients(Recipe recipe)
        {
            List<IngredientDetail> result = new List<IngredientDetail>();
            foreach (var item in recipe.IngIdAndAmount)
            {
                result.Add(new IngredientDetail() { Ingredient = _unitOfWork.Ingredients.GetById(item.Key), Amount = item.Value });
            }
            return result;
        }
        private void RestoreCategoriesInRecipes()
        {
            foreach(var a in _unitOfWork.Recipes.GetAll())
            {
                a.Category = RestoreCategory(a);
            }
        }
        private Category RestoreCategory(Recipe recipe)
        {
            return _unitOfWork.Categories.GetAll().SingleOrDefault(x => x.Id == recipe.CategoryId);
        }

        public bool TryCreateRecipe(Recipe recipe) // returns reference for future adding to category
        {
            CreateRecipe(recipe);
            bool result = _unitOfWork.Recipes.GetById(recipe.Id) != null ? true :  false;
            return result;
        }

        public void CreateRecipe(Recipe recipe)
        {
            var checker = _unitOfWork.Recipes.GetAll().SingleOrDefault(x => string.Equals(x.Name, recipe.Name, StringComparison.OrdinalIgnoreCase));
            if (checker != null)
            {
                Console.WriteLine($"Recipe {checker.Name} : {checker.Id} already exists");
                return;
            }
            if (string.IsNullOrEmpty(recipe.Id))
            {
                recipe.Id = Guid.NewGuid().ToString();
            }
            _unitOfWork.Recipes.Add(recipe);
            _unitOfWork.Save();
        }

        public Recipe PrepareRecipe(string name, string description, List<string> steps)
        {
            return new Recipe() { Name = name, Description = description, Steps = steps };
        }
        public void AddIngredientToRecipe(Recipe recipe, string ingredientName, string denomination, double amount)
        {
            var ingred = _unitOfWork.Ingredients.GetAll().SingleOrDefault(x => string.Equals(x.Name, ingredientName, StringComparison.OrdinalIgnoreCase) &&
                                                                      string.Equals(x.Denomination, denomination, StringComparison.OrdinalIgnoreCase));
            if (ingred == null)
            {
                ingred = new Ingredient { Id = Guid.NewGuid().ToString(), Name = ingredientName, Denomination = denomination };
                _unitOfWork.Ingredients.Add(ingred);
            }
            var ingDetail = new IngredientDetail() { Ingredient = ingred, Amount = amount };

            var checkerDetail = recipe.Ingredients.SingleOrDefault(x => x.Ingredient == ingDetail.Ingredient);
            if (checkerDetail != null)
            {
                checkerDetail.Amount += ingDetail.Amount;
                recipe.IngIdAndAmount[checkerDetail.Ingredient.Id] += amount;
            }
            else
            {
                recipe.Ingredients.Add(ingDetail);
                recipe.IngIdAndAmount.Add(ingDetail.Ingredient.Id, ingDetail.Amount);
            }
            _unitOfWork.Save();
        }

        public void SetCategoryInRecipe(Category category, Recipe recipe)
        {
            var retrieved = _unitOfWork.Recipes.GetAll().SingleOrDefault(x => x.CategoryId == recipe.CategoryId);
            if (retrieved != null)
            {
                Console.WriteLine($"Recipe {recipe.Name} is already in category");
                return;
            }
            else
            {
                recipe.CategoryId = category.Id;
                recipe.Category = category;
                _unitOfWork.Save();
            }
        }
    }
}
