using task2.UnitsOfWork;

namespace task2.Controllers
{
    class IngredientController : BaseController // turned out it wasn't needed
    {
        public IngredientController(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        //public Ingredient CreateAndGetIngredient(string ingName , string denomination)
        //{
        //    var getIng = _unitOfWork.Ingredients.SingleOrDefault(x => x.Name == ingName);
        //    if(getIng == null)
        //    {
        //        getIng = new Ingredient() { ID = Guid.NewGuid().ToString(), Name = ingName, Denomination = denomination };
        //        _unitOfWork.Ingredients.Add(getIng);
        //    }
        //    return getIng;
        //}
        //public void RemoveIngredient(Ingredient ing)
        //{
        //    _unitOfWork.Ingredients.Remove(ing);
        //}
        //public void RemoveIngredient(string ingName)
        //{
        //    Ingredient upForRemoval = _unitOfWork.Ingredients.SingleOrDefault(x => x.Name.Contains(ingName));
        //    double ingNameLength = ingName.Length;
        //    double foundLength = upForRemoval.Name.Length;
        //    if (ingNameLength / foundLength > 0.7) _unitOfWork.Ingredients.Remove(upForRemoval); // 0.7 is up for consideration
        //}

    }
}
