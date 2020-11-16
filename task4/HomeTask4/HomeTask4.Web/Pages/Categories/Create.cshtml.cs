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
    public class CreateModel : PageModel
    {
        private CategoryController _categoryController;
        [BindProperty]
        public Category Created {get;set;}

        public CreateModel(CategoryController categoryController)
        {
            Created = new Category();
            _categoryController = categoryController;
        }
        public void OnGet(int? Parentid)
        {
            Created.ParentId = Parentid;
        }
        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (ModelState.IsValid){
                try
                {
                    await _categoryController.CreateCategoryAsync(Created);
                    return RedirectToPage("Index", new { Id = Created.ParentId });
                }catch(Exception)
                {
                    return RedirectToPage("/Error");
                }
            }
            return Page();
        }
    }
}
