using System;
using System.Collections.Generic;
using System.Text;
using HomeTask4.SharedKernel.Interfaces;

namespace HomeTask4.Core.Controllers
{
    public class MeasureController : BaseController
    {
        public MeasureController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
