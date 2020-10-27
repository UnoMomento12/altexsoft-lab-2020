using HomeTask4.SharedKernel;
using System.Collections.Generic;

namespace HomeTask4.Core.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        //---------------------------------------------------------------//
        public int? ParentId { get; set; }
        public virtual Category Parent { get; set; }
        //---------------------------------------------------------------//
        public virtual List<Category> Categories { get; set; }
        public virtual List<Recipe> Recipes { get; set; }
    }
}
