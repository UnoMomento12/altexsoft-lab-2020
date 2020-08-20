using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace task2.Models
{
    class Recipe : BaseModel
    {
        public string Description { get; set; }
        public List<string> Steps { get; set; } = new List<string>();
        public Dictionary<string, double> IngIdAndAmount { get; set; } = new Dictionary<string, double>(); // ingredient id and amount of it

        [JsonIgnore]
        public List<IngredientDetail> Ingredients { get; set; } = new List<IngredientDetail>();
        
        
    }
}
