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
    public class EditModel : PageModel
    {
        private IngredientController _ingredientController;
        [BindProperty]
        public Ingredient toEdit { get; set; } 

        public EditModel(IngredientController ingredientController)
        {
            _ingredientController = ingredientController;
        }
        public async Task OnGetAsync(int? Id)
        {
            if (Id != null)
            {
                toEdit = await _ingredientController.GetIngredientByIdAsync((int) Id);
            }
        }
        public async Task<IActionResult> OnPostEditAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _ingredientController.UpdateIngredientAsync(toEdit);
                    return RedirectToPage("Index");
                }
                catch (Exception)
                {
                    return RedirectToPage("/Error");
                }
            }
            return Page();
        }
    }
}
