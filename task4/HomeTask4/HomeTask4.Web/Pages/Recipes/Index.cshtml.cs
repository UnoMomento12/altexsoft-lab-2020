using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeTask4.Core.Controllers;
using HomeTask4.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomeTask4.Web.Pages.Recipes
{
    public class IndexModel : PageModel
    {
        private RecipeController _recipeController;
        public List<Recipe> Recipes { get; set; }
        public IndexModel(RecipeController recipeController)
        {
            _recipeController = recipeController;
        }
        public async Task OnGetAsync()
        {
            Recipes = await _recipeController.GetAllRecipesAsync();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int deleteId)
        {
            try
            {
                await _recipeController.DeleteRecipeByIdAsync(deleteId);
            }
            catch (Exception)
            {
                return RedirectToPage("/Error");
            }
            return RedirectToPage("Index");
        }
    }
}
