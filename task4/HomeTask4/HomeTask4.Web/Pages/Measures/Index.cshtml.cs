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
            await _measureController.DeleteMeasureByIdAsync(deleteId);
            return RedirectToPage("Index");
        }
    }
}
