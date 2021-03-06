﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace task2.Models
{
    class Recipe : BaseModel
    {
        public string CategoryId { get; set; }
        public string Description { get; set; }
        public List<string> Steps { get; set; } = new List<string>();
        public Dictionary<string, double> IngIdAndAmount { get; set; } = new Dictionary<string, double>(); // ingredient id and amount of it

        [JsonIgnore]
        public List<IngredientDetail> Ingredients { get; set; } = new List<IngredientDetail>();
        [JsonIgnore]
        public Category Category { get; set; }

    }
}
