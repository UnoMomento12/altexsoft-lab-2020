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
        [BindProperty(SupportsGet =true)]
        public int? ParentId { get; set; }
        [BindProperty]
        public Category Created {get;set;}

        public CreateModel(CategoryController categoryController)
        {
            Created = new Category();
            _categoryController = categoryController;
        }
        public async Task OnGetAsync()
        {
        }
        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (ModelState.IsValid){
                try
                {
                    Created.ParentId = ParentId;
                    await _categoryController.CreateCategoryAsync(Created);
                    return RedirectToPage("Index", new { Id = ParentId });
                }catch(Exception e)
                {
                    return RedirectToPage("/Error");
                }
            }
            return Page();
        }
    }
}
