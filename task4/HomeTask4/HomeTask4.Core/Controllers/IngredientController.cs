
using HomeTask4.SharedKernel.Interfaces;

namespace HomeTask4.Core.Controllers
{
    public class IngredientController : BaseController // turned out it wasn't needed
    {
        public IngredientController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
