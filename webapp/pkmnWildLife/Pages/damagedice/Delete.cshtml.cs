using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.damagedice
{
    public class DeleteModel : PageModel
    {
        private readonly pkmnWildLife.Data.ApplicationDbContext _context;

        public DeleteModel(pkmnWildLife.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public DamageDice DamageDice { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.DamageDice == null)
            {
                return NotFound();
            }

            var damagedice = await _context.DamageDice.FirstOrDefaultAsync(m => m.ID == id);

            if (damagedice == null)
            {
                return NotFound();
            }
            else 
            {
                DamageDice = damagedice;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null || _context.DamageDice == null)
            {
                return NotFound();
            }
            var damagedice = await _context.DamageDice.FindAsync(id);

            if (damagedice != null)
            {
                DamageDice = damagedice;
                _context.DamageDice.Remove(DamageDice);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
