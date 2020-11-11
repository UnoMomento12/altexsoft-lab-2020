using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeTask4.Core.Controllers;
using HomeTask4.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HomeTask4.Web.Pages.Recipes
{
    public class EditModel : PageModel
    {
        private RecipeController _recipeController;
        private MeasureController _measureController;
        private CategoryController _categoryController;
        private IngredientDetailController _ingredientDetailController;
        private RecipeStepController _recipeStepController;
        public SelectList Measures { get; set; }
        public SelectList Categories { get; set; }
        [BindProperty]
        public Recipe toEdit { get; set; } 

        public EditModel(RecipeController recipeController, 
            MeasureController measureController, 
            CategoryController categoryController , 
            IngredientDetailController ingredientDetailController,
            RecipeStepController recipeStepController)
        {
            _recipeController = recipeController;
            _measureController = measureController;
            _categoryController = categoryController;
            _ingredientDetailController = ingredientDetailController;
            _recipeStepController = recipeStepController;
        }
        public async Task OnGetAsync(int? Id)
        {
            if (Id != null)
            {
                toEdit = await _recipeController.GetRecipeByIdAsync((int) Id);
                toEdit.Steps = toEdit.Steps.OrderBy(x => x.StepNumber).ToList();
                Measures = new SelectList( await _measureController.GetAllMeasuresAsync(),"Id", "Name");
                Categories = new SelectList(await _categoryController.GetAllCategoriesAsync(), "Id", "Name");
            }
        }
        public async Task<IActionResult> OnPostEditAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _recipeController.UpdateRecipeAsync(toEdit);
                    return RedirectToPage("Index");
                }
                catch (Exception)
                {
                    return RedirectToPage("/Error");
                }
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAddIngredientAsync(string newIngredient, double newAmount, int measureId)
        {
            try
            {
                await _recipeController.AddIngredientToRecipeAsync(toEdit, newIngredient, newAmount, measureId);
            }
            catch (Exception)
            {
                return RedirectToPage("/Error");
            }
            return RedirectToPage("/Recipes/Edit", new {Id = toEdit.Id });
        }

        public async Task<IActionResult> OnPostDeleteIngredientAsync(int recipeId, int ingredientId)
        {
            await _ingredientDetailController.DeleteIngredientDetailByIds(recipeId, ingredientId);
            return RedirectToPage("Edit", new { Id = toEdit.Id });
        }
        public async Task<IActionResult> OnPostDeleteStepAsync(int stepId)
        {
            await _recipeStepController.DeleteStepAsync(stepId);
            return RedirectToPage("Edit", new { Id = toEdit.Id });
        }

        public async Task<IActionResult> OnPostAddStepAsync(string description)
        {
            try
            {
                await _recipeStepController.AddStepToRecipeAsync(toEdit, description);
            }
            catch (Exception)
            {
                return RedirectToPage("/Error");
            }
            return RedirectToPage("/Recipes/Edit", new { Id = toEdit.Id });
        }

    }
}
