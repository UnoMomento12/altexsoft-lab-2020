using HomeTask4.SharedKernel;
using System.Collections.Generic;

namespace HomeTask4.Core.Entities
{
    public class Recipe : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        //------------------------------------------------//
        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }
        //------------------------------------------------//
        public virtual List<RecipeStep> Steps { get; set; }
        public virtual List<IngredientDetail> Ingredients { get; set; }


    }
}
