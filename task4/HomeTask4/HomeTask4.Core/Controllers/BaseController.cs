using HomeTask4.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
namespace HomeTask4.Core.Controllers
{
    public abstract class BaseController 
    {
        protected IUnitOfWork UnitOfWork;
        protected ILogger Logger;

        protected BaseController(IUnitOfWork unitOfWork , ILogger<BaseController> logger)
        {
            UnitOfWork = unitOfWork;
            Logger = logger;
        }
    }
}
