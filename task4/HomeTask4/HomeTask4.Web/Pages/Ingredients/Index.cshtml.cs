using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeTask4.Core.Controllers;
using HomeTask4.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomeTask4.Web.Pages.Ingredients
{
    public class IndexModel : PageModel
    {
        private IngredientController _ingredientController;
        public List<Ingredient> Ingredients { get; set; }
        public IndexModel(IngredientController ingredientController)
        {
            _ingredientController = ingredientController;
        }
        public async Task OnGetAsync()
        {
            Ingredients = await _ingredientController.GetAllIngredientsAsync();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int deleteId)
        {
            await _ingredientController.DeleteIngredientByIdAsync(deleteId);
            return RedirectToPage("Index");
        }
    }
}
