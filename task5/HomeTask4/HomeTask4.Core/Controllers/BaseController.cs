using HomeTask4.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
namespace HomeTask4.Core.Controllers
{
    public abstract class BaseController 
    {
        protected IUnitOfWork _unitOfWork;
        protected ILogger _logger;

        protected BaseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
