﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
namespace task2.Models
{
    class Category : BaseModel
    {

        public string ParentID { get; set; } = null;
        public List<string> RecipeIds { get; set; } = new List<string>();

        [JsonIgnore]
        public Category Parent { get; set; } = null;
        [JsonIgnore]
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();

    }
}
