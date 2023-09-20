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
    public class DetailsModel : PageModel
    {
        private readonly pkmnWildLife.Data.ApplicationDbContext _context;

        public DetailsModel(pkmnWildLife.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
