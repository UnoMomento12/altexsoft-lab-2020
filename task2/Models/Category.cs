using System.Collections.Generic;
using Newtonsoft.Json;
namespace task2.Models
{
    class Category : BaseModel
    {

        public string ParentId { get; set; } 
        public List<string> RecipeIds { get; set; } = new List<string>();

        [JsonIgnore]
        public Category Parent { get; set; }
        [JsonIgnore]
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();

    }
}
