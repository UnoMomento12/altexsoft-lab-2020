using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeTask4.Core.Controllers;
using HomeTask4.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomeTask4.Web.Pages.Categories
{
    public class EditModel : PageModel
    {
        private CategoryController _categoryController;
        [BindProperty]
        public Category toEdit { get; set; } 

        public EditModel(CategoryController categoryController)
        {
            _categoryController = categoryController;
        }
        public async Task OnGetAsync(int? Id)
        {
            if (Id != null)
            {
                toEdit = await _categoryController.GetCategoryByIdAsync((int) Id);
            }
        }
        public async Task<IActionResult> OnPostEditAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _categoryController.UpdateCategoryAsync(toEdit);
                    return RedirectToPage("Index", new { Id = toEdit.ParentId });
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
