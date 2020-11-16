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
    public class IndexModel : PageModel
    {
        private CategoryController _categoryController;
        public List<Category> ListedSubcategories { get; set; }
        public Category CurrentCategory { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }
        public IndexModel(CategoryController categoryController)
        {
            CurrentCategory = null;
            _categoryController = categoryController;
        }
        public async Task OnGetAsync()
        {
            if (Id != null)
            {
                CurrentCategory = await _categoryController.GetCategoryByIdAsync((int) Id);
                ListedSubcategories = CurrentCategory.Categories;
            }
            else
            {
                CurrentCategory = null;
                ListedSubcategories = await _categoryController.GetCategoriesByParentId(null);
            }
        }
        public async Task<IActionResult> OnPostDeleteAsync(int deleteId)
        {
            try
            {
                await _categoryController.DeleteCategoryByIdAsync(deleteId);
            } catch(Exception)
            {
                return RedirectToPage("/Error");
            }
            return RedirectToPage("Index", new { Id = CurrentCategory?.Id});
        }
    }
}
