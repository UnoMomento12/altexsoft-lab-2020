using System;
using HomeTask4.Core.Entities;
using HomeTask4.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using HomeTask4.Core.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using HomeTask4.SharedKernel;

namespace HomeTask4.Cmd
{
    enum NavigatorItem
    {
        Category = 1,
        Recipe = 2
    }
    class Program
    {
        private static ILogger<Program> _logger;
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            _logger = host.Services.GetRequiredService<ILogger<Program>>();
            RecipeController recCont = host.Services.GetRequiredService<RecipeController>();
            CategoryController catCont = host.Services.GetRequiredService<CategoryController>();
            RecipeStepController rsCont = host.Services.GetRequiredService<RecipeStepController>();
            NavigationController navig = host.Services.GetRequiredService<NavigationController>();
            await navig.StartNavigator();
            while (true)
            {
                WriteNavigator(navig);
                MenuStart();
                string input = Console.ReadLine().ToLower().Trim();
                if (input == "exit") break;
                int option;
                if (int.TryParse(input, out option))
                {
                    await ProcessNumberAsync(option, navig);
                }
                else if (input == "addrecipe")
                {
                    await ProcessRecipeAsync(recCont, rsCont, navig);
                }
                else if (input == "addcategory")
                {
                    await ProcessCategoryAsync(catCont, navig);
                }
                else
                {
                    Console.WriteLine("Unknown instruction!");
                    Console.ReadKey();
                }
            }
        }

        private static async Task ProcessNumberAsync(int option, NavigationController a)
        {
            if (option < 0) 
            { 
                await a.GoBack();
            }
            else if (a.InCategoriesBounds(option)) 
            { 
                await a.MoveToCategory(option); 
            }
            else if (a.InRecipesBounds(option))
            {
                List<BaseEntity> list = a.SubItems;
                WriteRecipe(list[option] as Recipe);
            }
        }

        private static async Task ProcessRecipeAsync(RecipeController recCont, RecipeStepController rsCont, NavigationController navig)
        {
            NavigatorItem item = NavigatorItem.Recipe;
            Category targetCategory = SetTargetCategory(navig, item);
            Recipe recipeToAdd;
            bool result = false;
            recipeToAdd = FormRecipe(recCont);
            recCont.SetCategoryInRecipe(targetCategory, recipeToAdd);
            try
            {
                result = await recCont.TryCreateRecipeAsync(recipeToAdd);
            } 
            catch (Exception)
            {
                Console.WriteLine("Creating recipe is not possible!");
                Console.ReadKey();
                return;
            }
            if (result)
            {
                await rsCont.AddStepsAsync(GatherSteps(recipeToAdd));
                await FormIngredientListAsync(recipeToAdd, recCont);
                await navig.UpdateSubItems();
                Console.WriteLine("Recipe created succesfully!");
            }
        }
        private static Recipe FormRecipe(RecipeController recCont)
        {
            Console.Write("Enter recipe name:");
            string name = Console.ReadLine();
            Console.Write("Enter recipe description:");
            string description = Console.ReadLine();
            return recCont.PrepareRecipe(name, description);
        }
        private static List<RecipeStep> GatherSteps(Recipe targetRecipe)
        {
            List<RecipeStep> steps = new List<RecipeStep>();
            Console.WriteLine("Enter sequence of recipe steps or \"-1\" to stop recording ");
            int index = 1;
            while (true)
            {
                string step = Console.ReadLine().Trim();
                if (step == "-1") break;
                steps.Add(new RecipeStep { StepNumber = index, Description = step, RecipeId = targetRecipe.Id });
                index++;
            }
            return steps;
        }


        private static async Task FormIngredientListAsync(Recipe recipeToAdd, RecipeController recCont)
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
                    parsed = double.TryParse(Console.ReadLine().Trim(), out amount);
                    if (parsed && amount == -1)
                    {
                        wasBreaked = true;
                        break;
                    }
                    else if (parsed)
                    {
                        break;
                    }
                }
                if (wasBreaked) break;
                Console.Write("Measured in:");
                string measure = Console.ReadLine().Trim();
                if (measure == "-1") break;
                try
                {
                    await recCont.AddIngredientToRecipeAsync(recipeToAdd, name, measure, amount);
                } 
                catch (Exception outer)
                {
                    _logger.LogError(outer.Message);
                    Console.WriteLine("Ingredient wasn't added to recipe, please try again");
                }
                
            }
        }
        private static async Task ProcessCategoryAsync(CategoryController catCont, NavigationController navig)
        {
            NavigatorItem item = NavigatorItem.Category;
            Category targetCategory = SetTargetCategory(navig, item);

            Console.Write("Enter Category Name:");
            string name = Console.ReadLine().Trim();

            bool result = false;
            try
            {
                result = await catCont.TryCreateCategoryAsync(name, targetCategory?.Id);
            } 
            catch (Exception)
            {
                Console.WriteLine("Creating category is not possible!");
                Console.ReadKey();
                return;
            }
            if (result)
            {
                Console.WriteLine("Category created succesfully!");
                await navig.UpdateSubItems();
            } 
        }
        private static Category SetTargetCategory(NavigationController navig, NavigatorItem item)
        {
            Category result = null;
            Category Current = navig.GetCurrent();

            if (navig.ItemCount == 0)
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

                System.ConsoleKey answer;
                while (true)
                {
                    answer = Console.ReadKey().Key;
                    if (answer == ConsoleKey.Y)
                    {
                        choice = true;
                        result = Current;
                        break;
                    }
                    else if (answer == ConsoleKey.N)
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
            if (!choice)
            {
                Console.Write($"Please enter an id of a category where created {item} will be stored:");
                while (int.TryParse(Console.ReadLine().Trim(), out categoryID) == false || navig.GetCategory(categoryID) == null)
                {
                    Console.Write("Wrong category id, please enter id again:");
                }
                result = navig.GetCategory(categoryID);
            }
            return result;
        }

        private static void WriteRecipe(Recipe recipe)
        {
            Console.WriteLine("-------------------------------------------");
            PrintColored($"Recipe name: {recipe.Name}", ConsoleColor.Green);
            Console.WriteLine("Ingredients:");
            foreach (var a in recipe.Ingredients)
            {
                Console.WriteLine($"{a.Ingredient.Name} :  {a.Amount} {a.Measure.Name}");
            }
            Console.WriteLine("Steps to cook:");
            for (int i = 0; i < recipe.Steps?.OrderBy(x => x.StepNumber).Count(); i++)
            {
                PrintColored($"{i + 1}. {recipe.Steps[i].Description}", ConsoleColor.Cyan);
            }
            PrintColored($"Description: {recipe.Description}", ConsoleColor.Yellow);
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Press any button to close recipe!");
            Console.ReadKey();
        }

        private static void WriteNavigator(NavigationController navigator)
        {
            Console.Clear();
            Console.WriteLine("Navigator!");
            if (navigator.CurrentCategory == null)
            {
                WriteRootNavigator(navigator);
            }
            else
            {
                WriteFullNavigator(navigator);
            }
        }

        private static void WriteFullNavigator(NavigationController navigator)
        {
            if (navigator.ItemCount == 0)
            {
                Console.WriteLine($"Category {navigator.CurrentCategory.Name} is empty!");
                return;
            }
            Console.WriteLine($"Subcategories and recipes in {navigator.CurrentCategory.Name} category.");
            WriteRootNavigator(navigator);
            Console.WriteLine("Recipes:");
            for (int b = navigator.RecipesStart; b < navigator.ItemCount; b++)
            {
                Console.WriteLine($"    {b}. {(navigator.SubItems[b] as Recipe)?.Name}");
            }
        }
        private static void WriteRootNavigator(NavigationController navigator)
        {
            Console.WriteLine("Categories:");
            for (int i = 0; i < navigator.RecipesStart; i++)
            {
                Console.WriteLine($"{i}. {(navigator.SubItems[i] as Category)?.Name}");
            }
        }
        private static void PrintColored(string message, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
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

        /// <summary>
        /// This method should be separate to support EF command-line tools in design time
        /// https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dbcontext-creation
        /// </summary>
        /// <param name="args"></param>
        /// <returns><see cref="IHostBuilder" /> hostBuilder</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
               .ConfigureServices((context, services) =>
               {
                   services.AddInfrastructure(context.Configuration.GetConnectionString("Default"));
               })
               .ConfigureLogging(config =>
               {
                   config.ClearProviders();
                   config.AddDebug();
               });

    }
}
