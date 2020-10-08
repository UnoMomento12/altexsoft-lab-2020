using HomeTask4.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
namespace HomeTask4.Core.Controllers
{
    public class IngredientDetailController : BaseController
    {
        public IngredientDetailController(IUnitOfWork unitOfWork, ILogger<IngredientDetailController> logger) : base(unitOfWork)
        {
            _logger = logger;
        }
    }
}
