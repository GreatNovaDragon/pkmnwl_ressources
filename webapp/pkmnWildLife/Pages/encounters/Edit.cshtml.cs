using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.encounters
{
    public class EditModel : PageModel
    {
        private readonly pkmnWildLife.Data.ApplicationDbContext _context;

        public EditModel(pkmnWildLife.Data.ApplicationDbContext context)
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

            var encounter =  await _context.Encounters.FirstOrDefaultAsync(m => m.ID == id);
            if (encounter == null)
            {
                return NotFound();
            }
            Encounter = encounter;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Encounter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EncounterExists(Encounter.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool EncounterExists(string id)
        {
          return (_context.Encounters?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
