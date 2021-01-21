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
    public class EditModel : PageModel
    {
        private MeasureController _measureController;
        [BindProperty]
        public Measure toEdit { get; set; } 

        public EditModel(MeasureController measureController)
        {
            _measureController = measureController;
        }
        public async Task OnGetAsync(int? Id)
        {
            if (Id != null)
            {
                toEdit = await _measureController.GetMeasureByIdAsync((int) Id);
            }
        }
        public async Task<IActionResult> OnPostEditAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _measureController.UpdateMeasureAsync(toEdit);
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
