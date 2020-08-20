﻿using task2.Models;
using task2.DataManager;
namespace task2.Repositories
{
    class IngredientRepository :Repository<Ingredient>
    {
        public IngredientRepository(IDataManager dataManager, Entities ent) : base(dataManager) 
        {
            ent.Ingredients = _items;
        }

        
    }
}
