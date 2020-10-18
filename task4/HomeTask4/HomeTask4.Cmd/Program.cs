using System;
using HomeTask4.Core.Entities;
using HomeTask4.Infrastructure.Data;
using HomeTask4.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using HomeTask4.Core.Controllers;
using System.Collections.Generic;
using HomeTask4.SharedKernel.Interfaces;
using System.Threading.Tasks;
using HomeTask4.Core.Navigator;
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
            Navigator navig = host.Services.GetRequiredService<Navigator>();
            await navig.StartNavigator();
            while (true)
            {
                navig.WriteNavigator();
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

        private static async Task ProcessNumberAsync(int option, Navigator a)
        {
            if (option < 0) await a.GoBack();
                else  await a.MoveTo(option);
        }

        private static async Task ProcessRecipeAsync(RecipeController recCont, RecipeStepController rsCont, Navigator navig)
        {
            NavigatorItem item = NavigatorItem.Recipe;
            Category targetCategory = SetTargetCategory(navig, item);
            Recipe recipeToAdd;
            bool result;
            recipeToAdd = FormRecipe(recCont);
            recCont.SetCategoryInRecipe(targetCategory, recipeToAdd);
            result = await recCont.TryCreateRecipeAsync(recipeToAdd);
            if (result)
            {
                await rsCont.AddStepsAsync(GatherSteps(recipeToAdd));
                await FormIngredientListAsync(recipeToAdd, recCont);
                await navig.UpdateSubItems();
                Console.WriteLine("Recipe created succesfully!");
            }
            else
            {
                Console.WriteLine("Creating recipe is not possible!");
                Console.ReadKey();
                return;
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
                } catch (Exception outer)
                {
                    _logger.LogInformation(outer.Message);
                    Console.WriteLine("Ingredient wasn't added to recipe, please try again");
                }
                
            }
        }
        private static async Task ProcessCategoryAsync(CategoryController catCont, Navigator navig)
        {
            NavigatorItem item = NavigatorItem.Category;
            Category targetCategory = SetTargetCategory(navig, item);

            Console.Write("Enter Category Name:");
            string name = Console.ReadLine().Trim();
            bool result = await catCont.TryCreateCategoryAsync(name, targetCategory?.Id);
            if (result)
            {
                Console.WriteLine("Category created succesfully!");
                await navig.UpdateSubItems();
            } else
            {
                Console.WriteLine("Creating category is not possible!");
                Console.ReadKey();
                return;
            }
        }
        private static Category SetTargetCategory(Navigator navig, NavigatorItem item)
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
