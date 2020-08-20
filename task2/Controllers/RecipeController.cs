
using System.Collections.Generic;
using task2.UnitsOfWork;
using task2.Models;
using System;
using System.Linq;
namespace task2.Controllers
{
    class RecipeController : BaseController
    {
        public RecipeController(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public void Add(Recipe recipe)
        {
            recipe = CreateRecipe(recipe);
        }
        public Recipe CreateRecipe(string name, string description, List<string> steps)
        {
            return CreateRecipe(new Recipe() { ID = Guid.NewGuid().ToString(), Name = name, Description = description, Steps = steps });
        }
        public Recipe CreateRecipe(Recipe recipe) // returns reference for future adding to category
        {
            var checker = _unitOfWork.Recipes.SingleOrDefault(x => string.Equals(x.Name, recipe.Name, StringComparison.OrdinalIgnoreCase));
            if (checker != null)
            {
                Console.WriteLine("Recipe " + checker.Name + " : " + checker.ID + " already exists");
                return checker;
            }
                
            if (string.IsNullOrEmpty(recipe.ID))
            {
                recipe.ID = Guid.NewGuid().ToString();
            }
                

            _unitOfWork.Recipes.Add(recipe);
            _unitOfWork.Save();
            return recipe;
        }

        public void RemoveRecipe(string recipeId)
        {
            var item = _unitOfWork.Recipes.Get(recipeId);
            if (item == null)
            {
                Console.WriteLine("Recipe " + recipeId + " has not been found");
                return;
            }
            _unitOfWork.Recipes.Remove(item);
            _unitOfWork.Save();
        }


        public Recipe GetRecipe(string guid)
        {
            var recipe = _unitOfWork.Recipes.Get(guid);
            return recipe;
        }
        public void AddIngredientToRecipe(Recipe recipe, string ingredientName, double amount)
        {
            var ingred = _unitOfWork.Ingredients.SingleOrDefault(x => String.Equals(x.Name, ingredientName, StringComparison.OrdinalIgnoreCase));
            if (ingred == null)
            {
                ingred = new Ingredient { ID = Guid.NewGuid().ToString(), Name = ingredientName };
                _unitOfWork.Ingredients.Add(ingred);
            }
            var ingDetail = new IngredientDetail(ingred, amount);

            var checkerDetail = recipe.Ingredients.SingleOrDefault(x => x.Ingredient.ID == ingDetail.Ingredient.ID);
            if (checkerDetail != null)
            {
                checkerDetail.Amount += ingDetail.Amount;
                recipe.IngIdAndAmount[checkerDetail.Ingredient.ID] += amount;
            }
            else
            {
                recipe.Ingredients.Add(ingDetail);
                recipe.IngIdAndAmount.Add(ingDetail.Ingredient.ID, ingDetail.Amount);
            }
            _unitOfWork.Save();
        }

        public void AddSteps(Recipe recipe, List<string> steps)
        {
            recipe.Steps.AddRange(steps);
            _unitOfWork.Save();
        }
    }
}
