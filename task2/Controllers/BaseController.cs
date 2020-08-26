using task2.UnitsOfWork;
namespace task2.Controllers
{
    abstract class BaseController : IController
    {
        protected IUnitOfWork _unitOfWork;

        protected BaseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
