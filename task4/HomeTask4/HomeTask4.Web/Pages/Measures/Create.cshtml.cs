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
    public class CreateModel : PageModel
    {
        private MeasureController _measureController;
        [BindProperty]
        public Measure Created {get;set;}

        public CreateModel(MeasureController measureController)
        {
            Created = new Measure();
            _measureController = measureController;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (ModelState.IsValid){
                try
                {
                    await _measureController.CreateMeasureAsync(Created);
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
