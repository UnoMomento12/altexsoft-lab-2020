
using HomeTask4.SharedKernel;
using HomeTask4.SharedKernel.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeTask4.Core.Controllers
{
    public abstract class BaseController 
    {
        protected IUnitOfWork _unitOfWork;

        protected BaseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
