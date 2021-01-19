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
    public class CreateModel : PageModel
    {
        private RecipeController _recipeController;
        [BindProperty]
        public Recipe Created {get;set;}
        [BindProperty(SupportsGet =true)]
        public int? CategoryId { get; set; }

        public CreateModel(RecipeController recipeController)
        {
            Created = new Recipe();
            _recipeController = recipeController;
        }
        public void OnGet()
        {
            Created.CategoryId = CategoryId;
        }
        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (ModelState.IsValid){
                try
                {
                    await _recipeController.CreateRecipeAsync(Created);
                    return RedirectToPage("/Recipes/Index");
                }catch(Exception)
                {
                    return RedirectToPage("/Error");
                }
            }
            return Page();
        }
    }
}
