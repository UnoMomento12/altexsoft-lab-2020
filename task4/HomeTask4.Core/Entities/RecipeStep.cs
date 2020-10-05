using HomeTask4.SharedKernel;
namespace HomeTask4.Core.Entities
{
    public class RecipeStep : BaseEntity
    {
        public string Description { get; set; }
        public int StepNumber { get; set; }
        //---------------------------------------------//
        public int RecipeId { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
