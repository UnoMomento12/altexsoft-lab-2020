using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeTask4.Core.Entities;
using HomeTask4.SharedKernel.Interfaces;

namespace HomeTask4.Core.Controllers
{
    public class RecipeStepController : BaseController
    {
        public RecipeStepController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
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
