using task2.UnitsOfWork;
namespace task2.Controllers
{
    abstract class BaseController : IController
    {
        protected UnitOfWork _unitOfWork;

        protected BaseController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
