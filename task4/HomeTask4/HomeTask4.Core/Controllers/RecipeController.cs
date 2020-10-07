using System;
using System.Linq;
using HomeTask4.SharedKernel.Interfaces;
using HomeTask4.Core.Entities;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace HomeTask4.Core.Controllers
{
    public class RecipeController : BaseController
    {
        public RecipeController(IUnitOfWork unitOfWork, ILogger<RecipeController> logger) : base(unitOfWork)
        {
            _logger = logger;
        }
        public async Task<bool> TryCreateRecipe(Recipe recipe) 
        {
            try
            {
                await CreateRecipe(recipe);
            } catch (Exception e)
            {
                _logger.LogInformation(e.Message, e);
                return false;
            }
            
            bool result = await _unitOfWork.Repository.GetByIdAsync<Recipe>(recipe.Id) != null ? true :  false;
            return result;
        }

        public async Task CreateRecipe(Recipe recipe)
        {
            var checker = (await _unitOfWork.Repository.ListAsync<Recipe>()).SingleOrDefault(x => string.Equals(x.Name, recipe.Name, StringComparison.OrdinalIgnoreCase) && x.CategoryId == recipe.CategoryId);
            if (checker != null)
            {
                throw new Exception($"Recipe {checker.Name} : {checker.Id} already exists");
            }
            await _unitOfWork.Repository.AddAsync<Recipe>(recipe);
        }

        public Recipe PrepareRecipe(string name, string description)
        {
            return new Recipe() { Name = name, Description = description};
        }
        public async Task AddIngredientToRecipe(Recipe recipe, string ingredientName, string measure, double amount)
        {
            var ingred = (await _unitOfWork.Repository.ListAsync<Ingredient>()).SingleOrDefault(x => string.Equals(x.Name, ingredientName, StringComparison.OrdinalIgnoreCase));
            if (ingred == null)
            {
                ingred = new Ingredient { Name = ingredientName};
                await _unitOfWork.Repository.AddAsync<Ingredient>(ingred);
            }

            var measuredIn = (await _unitOfWork.Repository.ListAsync<Measure>()).SingleOrDefault(x => string.Equals(x.Name, measure, StringComparison.OrdinalIgnoreCase));
            if( measuredIn == null)
            {
                measuredIn = new Measure { Name = measure };
                await _unitOfWork.Repository.AddAsync<Measure>(measuredIn);
            }


            var ingDetail = new IngredientDetail() { RecipeId = recipe.Id, IngredientId = ingred.Id, Amount = amount , MeasureId = measuredIn.Id };

            var checkerDetail = (await _unitOfWork.Repository.ListAsync<IngredientDetail>()).FirstOrDefault(x => x.RecipeId == ingDetail.RecipeId && x.IngredientId == ingDetail.IngredientId);
            if (checkerDetail != null)
            {
                checkerDetail.Amount += ingDetail.Amount;
                await _unitOfWork.Repository.UpdateAsync<IngredientDetail>(checkerDetail);
            }
            else
            {
                await _unitOfWork.Repository.AddAsync<IngredientDetail>(ingDetail);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public void SetCategoryInRecipe(Category category, Recipe recipe)
        {
                recipe.CategoryId = category.Id;
                recipe.Category = category;
        }
    }
}
