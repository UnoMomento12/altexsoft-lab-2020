using System.Collections.Generic;
using task2.Models;
namespace task2.DataManager
{
    class Entities
    {
        public List<Ingredient> Ingredients { get; set; }
        public List<Category> Categories { get; set; }
        public List<Recipe> Recipes { get; set; }
    }
}
