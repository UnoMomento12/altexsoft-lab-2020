using task2.UnitsOfWork;
namespace task2.Controllers
{
    abstract class BaseController 
    {
        protected IUnitOfWork WorkingUnit;

        protected BaseController(IUnitOfWork unitOfWork)
        {
            WorkingUnit = unitOfWork;
        }
    }
}
