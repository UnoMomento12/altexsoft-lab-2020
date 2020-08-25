using System;
using task2.UnitsOfWork;
using task2.Models;
using System.Collections.Generic;
using task2.Controllers;

namespace task2
{
    enum NavigatorItem
    {
        Category = 1,
        Recipe = 2
    }
    class Program
    {
        static void Main(string[] args)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            RecipeController recCont = new RecipeController(unitOfWork);
            CategoryController catCont = new CategoryController(unitOfWork);
            Navigator navig = new Navigator(unitOfWork);

            while (true)
            {
                navig.WriteNavigator();
                MenuStart();
                string input = Console.ReadLine().ToLower().Trim();
                if (input == "exit") break;
                int option;
                bool isNumber = Int32.TryParse(input, out option);
                if (isNumber)
                {
                    ProcessNumber(option, navig);
                }
                else if (input == "addrecipe" )
                {
                    ProcessRecipe(recCont, catCont , navig);
                }
                else if( input == "addcategory"){
                    ProcessCategory(catCont, navig);
                }
                else
                {
                    Console.WriteLine("Unknown instruction!");
                    Console.ReadKey();
                }
            }
        }

        private static void ProcessRecipe(RecipeController recCont, CategoryController catCont, Navigator navig)
        {
            NavigatorItem item = NavigatorItem.Recipe;
            Category targetCategory = null;
            targetCategory =  SetTargetCategory(navig, item);
            Recipe recipeToAdd = FormRecipe();
            if(recCont.TryCreateRecipe(recipeToAdd))
            {
                Console.WriteLine("Recipe created succesfully!");
            }
            else
            {
                Console.WriteLine("Creating recipe is not possible!");
                Console.ReadKey();
                return;
            }
            FormIngredientList(recipeToAdd ,recCont);
            catCont.AddRecipeToCategory(targetCategory, recipeToAdd);
            navig.UpdateSubItems();
        }

        private static void FormIngredientList(Recipe recipeToAdd, RecipeController recCont)
        {
            Console.WriteLine($"Enter ingredients for recipe {recipeToAdd.Name} below:");
            Console.WriteLine("Or enter -1 in any field to stop adding ingredients");
            
            while (true)
            {
                Console.Write("Ingredient name:");
                string name = Console.ReadLine().Trim();
                if (name == "-1") break;

                double amount;
                bool wasBreaked = false;
                bool parsed = false;
                while (true)
                {
                    Console.Write("Amount:");
                    parsed = Double.TryParse(Console.ReadLine(), out amount);
                    if (parsed && amount == -1)
                    {
                        wasBreaked = true;
                        break;
                    }
                    else if (parsed)
                    {
                        break;
                    } else
                    {

                    }
                }
                if (wasBreaked) break;
                Console.Write("Measured in:");
                string denomination = Console.ReadLine().Trim();
                if (denomination == "-1") break;
                recCont.AddIngredientToRecipe(recipeToAdd, name, denomination, amount);
            }
        }

        private static Recipe FormRecipe()
        {
            Console.Write("Enter recipe name:");
            string name = Console.ReadLine();
            Console.Write("Enter recipe description:");
            string description = Console.ReadLine();
            List<string> steps = new List<string>();
            Console.WriteLine("Enter sequence of recipe steps or \"-1\" to stop recording ");
            bool recording = true;
            while (recording)
            {
                string step = Console.ReadLine().Trim();
                if (step == "-1") break;
                steps.Add(step);
            }
            return new Recipe() { Name = name, Description = description, Steps = steps };
        }

        private static void ProcessCategory(CategoryController catCont, Navigator navig)
        {
            NavigatorItem item = NavigatorItem.Category;
            Category targetCategory = null;
            targetCategory= SetTargetCategory(navig, item);
            Console.Write("Enter Category Name:");
            string name = Console.ReadLine().Trim();
            if (catCont.TryCreateCategory(name, targetCategory?.Id))
            {
                Console.WriteLine("Category created succesfully!");
            }
            else
            {
                Console.WriteLine("Creating category is not possible!");
                Console.ReadKey();
                return;
            }
            navig.UpdateSubItems();
        }

        private static void ProcessNumber(int option , Navigator a)
        {
            if (option < 0 ) a.GoBack();
            a.MoveTo(option);
        }
        private static void MenuStart()
        {
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Caution: trying recipes in this cook book may bring harm to your body!");
            Console.WriteLine("Please enter id of an item in navigator.");
            Console.WriteLine("Or negative number to go to the root category");
            Console.WriteLine("Or enter one of the commands below: ");
            Console.WriteLine("addcategory - to create an additional category;");
            Console.WriteLine("addrecipe - to create recipe in the category;");
        }
        private static Category SetTargetCategory(Navigator navig , NavigatorItem item)
        {
            Category result = null;
            Category Current = navig.GetCurrent();

            if (navig.GetSubItemsCount() == 0)
            {
                Console.WriteLine("This category doesn't have subcategories!");
                result = Current;
                return result;
            }

            int categoryID;
            bool choice = false;
            if (item == NavigatorItem.Category || (Current != null && item == NavigatorItem.Recipe))
            {
                Console.WriteLine($"Do you want to add {item} to current {Current?.Name} category? Y/N");
                
                string answer;
                while (true)
                {
                    answer = Console.ReadLine().Trim().ToLower();
                    if (answer == "y")
                    {
                        choice = true;
                        result = Current;
                        break;
                    }
                    else if (answer == "n")
                    {
                        choice = false;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Wrong answer. Please enter \"Y\" (Yes) or \"N\" (No) again");
                    }
                }
            }
            if(choice == false)
            {
                Console.Write($"Please enter an id of a category where created {item} will be stored:");
                while (Int32.TryParse(Console.ReadLine().Trim(), out categoryID) == false || navig.GetCategory(categoryID) == null)
                {
                    Console.Write("Wrong category id, please enter id again:");
                }
                result = navig.GetCategory(categoryID);
            }
            return result;
        }

    }
}
