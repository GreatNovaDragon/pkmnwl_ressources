using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.pokemon
{
    public class DeleteModel : PageModel
    {
        private readonly pkmnWildLife.Data.ApplicationDbContext _context;

        public DeleteModel(pkmnWildLife.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Pokemon Pokemon { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.Pokedex == null)
            {
                return NotFound();
            }

            var pokemon = await _context.Pokedex.FirstOrDefaultAsync(m => m.ID == id);

            if (pokemon == null)
            {
                return NotFound();
            }
            else 
            {
                Pokemon = pokemon;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null || _context.Pokedex == null)
            {
                return NotFound();
            }
            var pokemon = await _context.Pokedex.FindAsync(id);

            if (pokemon != null)
            {
                Pokemon = pokemon;
                _context.Pokedex.Remove(Pokemon);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
