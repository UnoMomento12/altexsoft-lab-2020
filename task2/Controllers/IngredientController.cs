using System.Collections.Generic;
using task2.UnitsOfWork;
using task2.Models;
using System;

namespace task2.Controllers
{
    class IngredientController : BaseController
    {
        public IngredientController(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void AddIngredient(string ingName)
        {
            _unitOfWork.Ingredients.Add(new Ingredient() { ID = Guid.NewGuid().ToString() , Name = ingName });
        }

        public void RemoveIngredient(Ingredient ing)
        {
            _unitOfWork.Ingredients.Remove(ing);
        }
        public void RemoveIngredient(string ingName)
        {
            Ingredient upForRemoval = _unitOfWork.Ingredients.SingleOrDefault(x => x.Name.Contains(ingName));
            double ingNameLength = ingName.Length;
            double foundLength = upForRemoval.Name.Length;
            if (ingNameLength / foundLength > 0.7) _unitOfWork.Ingredients.Remove(upForRemoval); // 0.7 is up for consideration
        }
    }
}
