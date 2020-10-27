using Microsoft.Extensions.Logging;
using HomeTask4.SharedKernel.Interfaces;

namespace HomeTask4.Core.Controllers
{
    public class IngredientController : BaseController 
    {
        public IngredientController(IUnitOfWork unitOfWork, ILogger<IngredientController> logger) : base(unitOfWork, logger)
        {
        }
    }
}
