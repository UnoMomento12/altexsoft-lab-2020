using System;
using System.Linq;
using HomeTask4.SharedKernel.Interfaces;
using HomeTask4.Core.Entities;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Castle.Core.Internal;
using System.Collections.Generic;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using HomeTask4.Core.Exceptions;
namespace HomeTask4.Core.Controllers
{
    public class RecipeController : BaseController
    {
        public RecipeController(IUnitOfWork unitOfWork, ILogger<RecipeController> logger) : base(unitOfWork, logger)
        {
        }
        public async Task<bool> TryCreateRecipeAsync(Recipe recipe) 
        {
            if (recipe == null) return false;
            try
            {
                await CreateRecipeAsync(recipe);
            } 
            catch (ArgumentNullException nullException)
            {
                Logger.LogError(nullException.Message);
                throw;
            }
            catch (EmptyFieldException emptyFieldException)
            {
                Logger.LogError(emptyFieldException.Message);
                throw;
            } 
            catch (EntryAlreadyExistsException entryAlreadyExistsException)
            {
                Logger.LogError(entryAlreadyExistsException.Message);
                throw;
            }
            bool result = await UnitOfWork.Repository.GetByIdAsync<Recipe>(recipe.Id) != null;
            return result;
        }

        public async Task CreateRecipeAsync(Recipe recipe)// everything is thrown here
        {
            if (recipe == null) throw new ArgumentNullException("Recipe reference is null.");
            if (recipe.Name.IsNullOrEmpty()) throw new EmptyFieldException("Recipe name is null or empty.");
            if (recipe.Description.IsNullOrEmpty()) throw new EmptyFieldException("Recipe description is null or empty.");
            var checker = (await UnitOfWork.Repository.FirstOrDefaultAsync<Recipe>(x => x.Name.ToLower() == recipe.Name.ToLower() && x.CategoryId == recipe.CategoryId));
            if (checker != null)
            {
                throw new EntryAlreadyExistsException($"Recipe {checker.Name} : {checker.Id} already exists");
            }
            await UnitOfWork.Repository.AddAsync<Recipe>(recipe);
        }

        public Recipe PrepareRecipe(string name, string description)
        {
            return new Recipe() { Name = name, Description = description};
        }
        public async Task AddIngredientToRecipeAsync(Recipe recipe, string ingredientName, string measure, double amount)
        {
            var measuredIn = (await UnitOfWork.Repository.FirstOrDefaultAsync<Measure>(x => x.Name.ToLower() ==  measure.ToLower()));
            if( measuredIn == null)
            {
                measuredIn = new Measure { Name = measure };
                await UnitOfWork.Repository.AddAsync<Measure>(measuredIn);
            }
            await AddIngredientToRecipeAsync(recipe, ingredientName, amount, measuredIn.Id);
        }
        public async Task AddIngredientToRecipeAsync(Recipe recipe, string ingredientName, double amount, int measureId)
        {
            if (recipe == null) throw new ArgumentNullException("Recipe reference is null.");
            if (ingredientName.IsNullOrEmpty()) throw new EmptyFieldException("IngredientName is empty.");

            var checkRecipe = (await UnitOfWork.Repository.FirstOrDefaultAsync<Recipe>(x => x.Name.ToLower() == recipe.Name.ToLower() && x.CategoryId == recipe.CategoryId && x.Id == recipe.Id));
            if (checkRecipe == null)
            {
                throw new  EntryNotFoundException($"Recipe {recipe.Name} : {recipe.Id} doesn't exist in Database.");
            }

            var ingred = (await UnitOfWork.Repository.FirstOrDefaultAsync<Ingredient>(x => x.Name.ToLower() == ingredientName.ToLower()));
            if (ingred == null)
            {
                ingred = new Ingredient { Name = ingredientName };
                await UnitOfWork.Repository.AddAsync<Ingredient>(ingred);
            }

            var ingDetail = new IngredientDetail() { RecipeId = checkRecipe.Id, IngredientId = ingred.Id, Amount = amount, MeasureId = measureId };

            var checkerDetail = (await UnitOfWork.Repository.FirstOrDefaultAsync<IngredientDetail>(x => x.RecipeId == ingDetail.RecipeId && x.IngredientId == ingDetail.IngredientId));
            if (checkerDetail != null)
            {
                checkerDetail.Amount += ingDetail.Amount;
                await UnitOfWork.Repository.UpdateAsync<IngredientDetail>(checkerDetail);
            }
            else
            {
                checkRecipe.Ingredients.Add(ingDetail);
                await UnitOfWork.Repository.UpdateAsync<Recipe>(checkRecipe);
            }
        }

        public void SetCategoryInRecipe(Category category, Recipe recipe)
        {
                recipe.CategoryId = category.Id;
                recipe.Category = category;
        }
        public Task<List<Recipe>> GetAllRecipesAsync()
        {
            return UnitOfWork.Repository.ListAsync<Recipe>();
        }
        public async Task DeleteRecipeByIdAsync(int id)
        {
            Recipe toDelete = await UnitOfWork.Repository.GetByIdAsync<Recipe>(id);
            if (toDelete == null)
            {
                throw new EntryNotFoundException("This recipe doesn't exist in database.");
            }
            await UnitOfWork.Repository.DeleteAsync<Recipe>(toDelete);
        }

        public async Task UpdateRecipeAsync(Recipe toUpdate)
        {
            var retrieved = await UnitOfWork.Repository.GetByIdAsync<Recipe>(toUpdate.Id);
            if (retrieved == null)
            {
                throw new EntryNotFoundException($"This {toUpdate.Name} recipe doesn't exist in database.");
            }
            retrieved.Name = toUpdate.Name;
            retrieved.Description = toUpdate.Description;
            retrieved.CategoryId = toUpdate.CategoryId;
            await UnitOfWork.Repository.UpdateAsync(retrieved);
        }
        public Task<Recipe> GetRecipeByIdAsync(int id)
        {
            return UnitOfWork.Repository.GetByIdAsync<Recipe>(id);
        }
    }
}
