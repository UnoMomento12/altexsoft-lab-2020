using HomeTask4.SharedKernel;
namespace HomeTask4.Core.Entities
{
    public class IngredientDetail : BaseEntity
    {
        public int RecipeId { get; set; }
        public double Amount { get; set; }
        //----------------------------------------------//
        public int IngredientId { get; set; }
        public virtual Ingredient Ingredient { get; set; }
        //---------------------------------------------//
        public int MeasureId { get; set; }
        public virtual Measure Measure { get; set; }
    }
}
