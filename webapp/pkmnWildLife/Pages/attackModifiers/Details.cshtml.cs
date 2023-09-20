using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.attackModifiers
{
    public class DetailsModel : PageModel
    {
        private readonly pkmnWildLife.Data.ApplicationDbContext _context;

        public DetailsModel(pkmnWildLife.Data.ApplicationDbContext context)
        {
            _context = context;
        }

      public AttackMod AttackMod { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.AttackModifiers == null)
            {
                return NotFound();
            }

            var attackmod = await _context.AttackModifiers.FirstOrDefaultAsync(m => m.ID == id);
            if (attackmod == null)
            {
                return NotFound();
            }
            else 
            {
                AttackMod = attackmod;
            }
            return Page();
        }
    }
}
