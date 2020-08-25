using task2.UnitsOfWork;
using task2.Models;
using System;
using System.Linq;
namespace task2.Controllers
{
    class RecipeController : BaseController
    {
        public RecipeController(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public bool TryCreateRecipe(Recipe recipe) // returns reference for future adding to category
        {
            CreateRecipe(recipe);
            bool result = _unitOfWork.Recipes.Get(recipe.Id) != null ? true :  false;
            return result;
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
    }
}
