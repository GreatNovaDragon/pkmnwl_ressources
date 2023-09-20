using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.encounters
{
    public class DeleteModel : PageModel
    {
        private readonly pkmnWildLife.Data.ApplicationDbContext _context;

        public DeleteModel(pkmnWildLife.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Encounter Encounter { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.Encounters == null)
            {
                return NotFound();
            }

            var encounter = await _context.Encounters.FirstOrDefaultAsync(m => m.ID == id);

            if (encounter == null)
            {
                return NotFound();
            }
            else 
            {
                Encounter = encounter;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null || _context.Encounters == null)
            {
                return NotFound();
            }
            var encounter = await _context.Encounters.FindAsync(id);

            if (encounter != null)
            {
                Encounter = encounter;
                _context.Encounters.Remove(Encounter);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
