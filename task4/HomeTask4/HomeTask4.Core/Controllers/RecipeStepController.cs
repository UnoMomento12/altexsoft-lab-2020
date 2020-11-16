using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Core.Internal;
using HomeTask4.Core.Entities;
using HomeTask4.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
using HomeTask4.Core.Exceptions;
namespace HomeTask4.Core.Controllers
{
    public class RecipeStepController : BaseController
    {
        public RecipeStepController(IUnitOfWork unitOfWork, ILogger<RecipeStepController> logger) : base(unitOfWork, logger)
        {
        }
        
        public async Task AddStepsAsync(List<RecipeStep> steps)
        {
            await UnitOfWork.Repository.AddRangeAsync<RecipeStep>(steps);
        }

        public async Task DeleteStepByIdAsync(int stepId)
        {
            RecipeStep toDelete = await UnitOfWork.Repository.GetByIdAsync<RecipeStep>(stepId);
            if (toDelete == null)
            {
                throw new EntryNotFoundException("This step doesn't exist in database.");
            }
            await UnitOfWork.Repository.DeleteAsync<RecipeStep>(toDelete);
        }
        public async Task AddStepToRecipeAsync(Recipe recipe, string description)
        {
            string errorMessage = "";
            if (recipe == null)
            {
                errorMessage = "Recipe reference is null.";
                Logger.LogError(errorMessage);
                throw new ArgumentNullException(errorMessage);
            }
            if (description.IsNullOrEmpty())
            {
                errorMessage = "Step Description is empty.";
                Logger.LogError(errorMessage);
                throw new EmptyFieldException(errorMessage);
            }
            var checkRecipe = (await UnitOfWork.Repository.FirstOrDefaultAsync<Recipe>(x => x.Name.ToLower() == recipe.Name.ToLower() && x.CategoryId == recipe.CategoryId && x.Id == recipe.Id));
            if (checkRecipe == null)
            {
                errorMessage = $"Recipe {recipe.Name} : {recipe.Id} doesn't exist in Database.";
                Logger.LogError(errorMessage);
                throw new EntryAlreadyExistsException (errorMessage);
            }
            var recipeStep = new RecipeStep() { RecipeId = checkRecipe.Id, Description=description, StepNumber = checkRecipe.Steps.Count + 1 };
            checkRecipe.Steps.Add(recipeStep);
            await UnitOfWork.Repository.UpdateAsync<Recipe>(checkRecipe);
           
        }

    }
}
