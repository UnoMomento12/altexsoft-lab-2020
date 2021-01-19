using HomeTask4.Core.Entities;
using HomeTask4.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using HomeTask4.Core.Exceptions;
namespace HomeTask4.Core.Controllers
{
    public class IngredientDetailController : BaseController
    {
        public IngredientDetailController(IUnitOfWork unitOfWork, ILogger<IngredientDetailController> logger) : base(unitOfWork, logger)
        {
        }
        public async Task DeleteIngredientDetailByIdsAsync(int recipeId, int ingredientId)
        {
            IngredientDetail retrieved = await UnitOfWork.Repository.FirstOrDefaultAsync<IngredientDetail>(x => x.RecipeId == recipeId && x.IngredientId == ingredientId);
            if(retrieved == null)
            {
                throw new EntryNotFoundException("This IngredientDetail doesn't exist in database.");
            }
            await UnitOfWork.Repository.DeleteAsync<IngredientDetail>(retrieved);
        }
    }
}
