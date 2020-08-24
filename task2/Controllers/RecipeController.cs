using task2.UnitsOfWork;
using task2.Models;
using System;
using System.Linq;
namespace task2.Controllers
{
    class RecipeController : BaseController
    {
        public RecipeController(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Recipe CreateAndGetRecipe(Recipe recipe) // returns reference for future adding to category
        {
            CreateRecipe(recipe);
            return recipe;
        }

        public void CreateRecipe(Recipe recipe)
        {
            var checker = _unitOfWork.Recipes.SingleOrDefault(x => string.Equals(x.Name, recipe.Name, StringComparison.OrdinalIgnoreCase));
            if (checker != null)
            {
                Console.WriteLine($"Recipe {checker.Name} : {checker.Id} already exists");
                recipe = checker;
                return;
            }
            if (string.IsNullOrEmpty(recipe.Id))
            {
                recipe.Id = Guid.NewGuid().ToString();
            }
            _unitOfWork.Recipes.Add(recipe);
            _unitOfWork.Save();
        }


        public void AddIngredientToRecipe(Recipe recipe, string ingredientName, string denomination, double amount)
        {
            var ingred = _unitOfWork.Ingredients.SingleOrDefault(x => String.Equals(x.Name, ingredientName, StringComparison.OrdinalIgnoreCase) && 
                                                                      String.Equals(x.Denomination, denomination, StringComparison.OrdinalIgnoreCase));
            if (ingred == null)
            {
                ingred = new Ingredient { Id = Guid.NewGuid().ToString(), Name = ingredientName, Denomination = denomination };
                _unitOfWork.Ingredients.Add(ingred);
            }
            var ingDetail = new IngredientDetail(ingred, amount);

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

        //public void AddSteps(Recipe recipe, List<string> steps)
        //{
        //    recipe.Steps.AddRange(steps);
        //    _unitOfWork.Save();
        //}

        //public void RemoveRecipe(string recipeId)
        //{
        //    var item = _unitOfWork.Recipes.Get(recipeId);
        //    if (item == null)
        //    {
        //        Console.WriteLine("Recipe " + recipeId + " has not been found");
        //        return;
        //    }
        //    _unitOfWork.Recipes.Remove(item);
        //    _unitOfWork.Save();
        //}

        //public Recipe GetRecipe(string guid)
        //{
        //    var recipe = _unitOfWork.Recipes.Get(guid);
        //    return recipe;
        //}

        //public void Add(Recipe recipe)
        //{
        //    recipe = CreateAndGetRecipe(recipe);
        //}
        //public Recipe CreateAndGetRecipe(string name, string description, List<string> steps)
        //{
        //    return CreateAndGetRecipe(new Recipe() { ID = Guid.NewGuid().ToString(), Name = name, Description = description, Steps = steps });
        //}
    }
}
