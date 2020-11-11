using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Core.Internal;
using HomeTask4.Core.Entities;
using HomeTask4.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
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

        public async Task DeleteStepAsync(int stepId)
        {
            RecipeStep toDelete = await UnitOfWork.Repository.GetByIdAsync<RecipeStep>(stepId);
            await UnitOfWork.Repository.DeleteAsync<RecipeStep>(toDelete);
        }
        public async Task AddStepToRecipeAsync(Recipe recipe, string description)
        {
            if (recipe == null) throw new ArgumentException("Recipe reference is null.");
            if (description.IsNullOrEmpty()) throw new ArgumentException("Description is empty.");
            var checkRecipe = (await UnitOfWork.Repository.FirstOrDefaultAsync<Recipe>(x => x.Name.ToLower() == recipe.Name.ToLower() && x.CategoryId == recipe.CategoryId && x.Id == recipe.Id));
            if (checkRecipe == null)
            {
                throw new ArgumentException($"Recipe {recipe.Name} : {recipe.Id} doesn't exist in Database.");
            }
            var recipeStep = new RecipeStep() { RecipeId = checkRecipe.Id, Description=description, StepNumber = checkRecipe.Steps.Count + 1 };
            checkRecipe.Steps.Add(recipeStep);
            await UnitOfWork.Repository.UpdateAsync<Recipe>(checkRecipe);
           
        }

    }
}
