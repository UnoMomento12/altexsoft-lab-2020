using HomeTask4.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
namespace HomeTask4.Core.Controllers
{
    public class MeasureController : BaseController
    {
        public MeasureController(IUnitOfWork unitOfWork, ILogger<MeasureController> logger) : base(unitOfWork)
        {
            _logger = logger;
        }
    }
}
