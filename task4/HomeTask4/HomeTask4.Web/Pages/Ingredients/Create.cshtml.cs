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
    public class CreateModel : PageModel
    {
        private IngredientController _ingredientController;
        [BindProperty]
        public Ingredient Created {get;set;}

        public CreateModel(IngredientController ingredientController)
        {
            Created = new Ingredient();
            _ingredientController = ingredientController;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (ModelState.IsValid){
                try
                {
                    await _ingredientController.CreateIngredientAsync(Created);
                    return RedirectToPage("Index");
                }catch(Exception)
                {
                    return RedirectToPage("/Error");
                }
            }
            return Page();
        }
    }
}
