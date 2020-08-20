using System;
using task2.UnitsOfWork;
using task2.DataManager;
using task2.Models;
using System.Collections.Generic;
using task2.Controllers;
using System.Linq;

namespace task2
{
    class Program
    {
        static void Main(string[] args)
        {
            Entities entities = new Entities();
            UnitOfWork unitOfWork = new UnitOfWork(entities);
            IngredientController ingCont = new IngredientController(unitOfWork);
            RecipeController recCont = new RecipeController(unitOfWork);
            CategoryController catCont = new CategoryController(unitOfWork);
            Navigator navig = new Navigator(unitOfWork);

            //ingCont.AddIngredient("Salt");

            //foreach(var a in unitOfWork.Ingredients.GetItems())
            //{
            //    Console.WriteLine("Guid: " + a.ID + ",name: " + a.Name);
            //}

            //Recipe z = recCont.CreateRecipe("Fuagra", "Receipt of fuagra", new List<string> { "Boil water", "Salt the water", "Throw in mushrooms" });
            //recCont.AddIngredientToRecipe(z, "water", 1000);
            //recCont.AddIngredientToRecipe(z, "salt", 50);
            //recCont.AddIngredientToRecipe(z, "salt", 20);
            //foreach(var wrap in unitOfWork.Recipes.GetItems())
            //{
            //    Console.WriteLine(wrap.ID + " : " + wrap.Name);
            //    Console.WriteLine(wrap.Description);
            //    foreach(var step in wrap.Steps)
            //    {
            //        Console.WriteLine(step);
            //    }
            //    foreach (var a in wrap.Ingredients)
            //    {
            //        Console.WriteLine(a.Ingredient.Name + " : " + a.Ingredient.ID + " : " + a.Amount); 
            //    }
            //}

            //Category myC = catCont.CreateCategory("Cakes");
            //Category subC = catCont.CreateCategory("Small cakes", myC.ID);
            //catCont.AddRecipeToCategory(myC, z);

            //foreach (var wrap in unitOfWork.Categories.GetItems())
            //{
            //    Console.WriteLine(wrap.ID + " : " + wrap.Name);
            //    Console.WriteLine(wrap.ParentID);
            //    Console.WriteLine(wrap.Parent?.Name);
            //    foreach (var recipe in wrap.Recipes)
            //    {
            //        Console.WriteLine(recipe.ID + " : " + recipe.Name);
            //    }
            //}

            navig.WriteNavigator();
            navig.MoveTo(0);
            navig.WriteNavigator();
            int idC;
            Int32.TryParse(Console.ReadLine(), out idC);
            navig.MoveTo(idC);
            unitOfWork.Save();
            Console.ReadKey();
            Console.WriteLine("Hello World!");
        }
    }
}
