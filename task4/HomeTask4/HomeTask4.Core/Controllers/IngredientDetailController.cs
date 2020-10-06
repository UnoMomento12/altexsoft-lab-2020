using HomeTask4.SharedKernel.Interfaces;

namespace HomeTask4.Core.Controllers
{
    public class IngredientDetailController : BaseController
    {
        public IngredientDetailController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
