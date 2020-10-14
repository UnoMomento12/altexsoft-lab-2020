using System;
using System.Linq;
using HomeTask4.SharedKernel.Interfaces;
using HomeTask4.Core.Entities;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Castle.Core.Internal;

namespace HomeTask4.Core.Controllers
{
    public class RecipeController : BaseController
    {
        public RecipeController(IUnitOfWork unitOfWork, ILogger<RecipeController> logger) : base(unitOfWork, logger)
        {
        }


        public Task<bool> TryCreateRecipeAsync(string name, string description, int? categoryId)
        {
            if (name.IsNullOrEmpty() || description.IsNullOrEmpty() || categoryId == null)
            {
                throw new ArgumentException("One of the fields is null or empty.");
            }

            return TryCreateRecipeAsync(new Recipe { Name = name, Description = description, CategoryId = categoryId });
        }
        public async Task<bool> TryCreateRecipeAsync(Recipe recipe) 
        {
            try
            {
                await CreateRecipeAsync(recipe);
            } catch (Exception e)
            {
                Logger.LogInformation(e.Message, e);
                return false;
            }
            
            bool result = await UnitOfWork.Repository.GetByIdAsync<Recipe>(recipe.Id) != null;
            return result;
        }

        public async Task CreateRecipeAsync(Recipe recipe)
        {
            if (recipe == null) throw new ArgumentNullException("Recipe reference is null.");
            if (recipe.Name.IsNullOrEmpty()) throw new ArgumentException("Name is empty.");
            var checker = (await UnitOfWork.Repository.FirstOrDefaultAsync<Recipe>(x => x.Name.ToLower() == recipe.Name.ToLower() && x.CategoryId == recipe.CategoryId));
            if (checker != null)
            {
                throw new ArgumentException($"Recipe {checker.Name} : {checker.Id} already exists");
            }
            await UnitOfWork.Repository.AddAsync<Recipe>(recipe);
        }

        public Recipe PrepareRecipe(string name, string description)
        {
            if(name.IsNullOrEmpty() || description.IsNullOrEmpty())
            {
                throw new ArgumentException("Name or description is empty");
            }
            return new Recipe() { Name = name, Description = description};
        }
        public async Task AddIngredientToRecipeAsync(Recipe recipe, string ingredientName, string measure, double amount)
        {
            if (ingredientName.IsNullOrEmpty()) throw new ArgumentException("IngredientName is empty.");

            var checkRecipe = (await UnitOfWork.Repository.FirstOrDefaultAsync<Recipe>(x => x.Name.ToLower() == recipe.Name.ToLower() && x.CategoryId == recipe.CategoryId && x.Id == recipe.Id));
            if (checkRecipe == null)
            {
                throw new ArgumentException($"Recipe {recipe.Name} : {recipe.Id} doesn't exist in Database.");
            }

            var ingred = (await UnitOfWork.Repository.FirstOrDefaultAsync<Ingredient>(x => x.Name.ToLower() == ingredientName.ToLower()));
            if (ingred == null)
            {
                ingred = new Ingredient { Name = ingredientName};
                await UnitOfWork.Repository.AddAsync<Ingredient>(ingred);
            }
            
            var measuredIn = (await UnitOfWork.Repository.FirstOrDefaultAsync<Measure>(x => x.Name.ToLower() ==  measure.ToLower()));
            if( measuredIn == null)
            {
                measuredIn = new Measure { Name = measure };
                await UnitOfWork.Repository.AddAsync<Measure>(measuredIn);
            }

            var ingDetail = new IngredientDetail() { RecipeId = recipe.Id, IngredientId = ingred.Id, Amount = amount , MeasureId = measuredIn.Id };

            var checkerDetail = (await UnitOfWork.Repository.FirstOrDefaultAsync<IngredientDetail>(x => x.RecipeId == ingDetail.RecipeId && x.IngredientId == ingDetail.IngredientId));
            if (checkerDetail != null)
            {
                checkerDetail.Amount += ingDetail.Amount;
                await UnitOfWork.Repository.UpdateAsync<IngredientDetail>(checkerDetail);
            }
            else
            {
                await UnitOfWork.Repository.AddAsync<IngredientDetail>(ingDetail);
            }
        }

        public void SetCategoryInRecipe(Category category, Recipe recipe)
        {
                recipe.CategoryId = category.Id;
                recipe.Category = category;
        }
    }
}
