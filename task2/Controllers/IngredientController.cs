﻿using task2.UnitsOfWork;

namespace task2.Controllers
{
    class IngredientController : BaseController // turned out it wasn't needed
    {
        public IngredientController(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
