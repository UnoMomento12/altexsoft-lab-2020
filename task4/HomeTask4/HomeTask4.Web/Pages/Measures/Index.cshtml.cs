using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeTask4.Core.Controllers;
using HomeTask4.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomeTask4.Web.Pages.Measures
{
    public class IndexModel : PageModel
    {
        private MeasureController _measureController;
        public List<Measure> Ingredients { get; set; }
        public IndexModel(MeasureController measureController)
        {
            _measureController = measureController;
        }
        public async Task OnGetAsync()
        {
            Ingredients = await _measureController.GetAllMeasuresAsync();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int deleteId)
        {
            try
            {
                await _measureController.DeleteMeasureByIdAsync(deleteId);
            }
            catch (Exception)
            {
                return RedirectToPage("/Error");
            }
            return RedirectToPage("Index");
        }
    }
}
