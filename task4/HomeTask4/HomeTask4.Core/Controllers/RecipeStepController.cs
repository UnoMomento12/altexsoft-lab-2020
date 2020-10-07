using System.Collections.Generic;
using System.Threading.Tasks;
using HomeTask4.Core.Entities;
using HomeTask4.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
namespace HomeTask4.Core.Controllers
{
    public class RecipeStepController : BaseController
    {
        public RecipeStepController(IUnitOfWork unitOfWork, ILogger<RecipeStepController> logger) : base(unitOfWork)
        {
            _logger = logger;
        }
        
        public async Task AddStepsAsync(List<RecipeStep> steps)
        {
            foreach( RecipeStep a in steps)
            {
                await _unitOfWork.Repository.AddAsync<RecipeStep>(a);
            }
        }

    }
}
