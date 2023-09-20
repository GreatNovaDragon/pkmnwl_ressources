using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.damagedice
{
    public class CreateModel : PageModel
    {
        private readonly pkmnWildLife.Data.ApplicationDbContext _context;

        public CreateModel(pkmnWildLife.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public DamageDice DamageDice { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.DamageDice == null || DamageDice == null)
            {
                return Page();
            }

            _context.DamageDice.Add(DamageDice);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
