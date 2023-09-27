using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;
using Ability = PokeApiNet.Ability;

namespace pkmnWildLife.Pages.pokemon
{
    public class DetailsModel : PageModel
    {
        private readonly pkmnWildLife.Data.ApplicationDbContext _context;

        public DetailsModel(pkmnWildLife.Data.ApplicationDbContext context)
        {
            _context = context;
        }

      public Pokemon Pokemon { get; set; } = default!; 
      public List<Learnsets> Learnsets { get; set; }
      
      public List<Data.Ability> Abilities { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.Pokedex == null)
            {
                return NotFound();
            }

            var pokemon = await _context.Pokedex.Include(p => p.learnset).ThenInclude(m => m.move).ThenInclude(m => m.type).Include(p => p.Abilities).FirstOrDefaultAsync(m => m.ID == id);
            if (pokemon == null)
            {
                return NotFound();
            }
            else 
            {
                Pokemon = pokemon;
                Learnsets = Pokemon.learnset;
                Abilities = Pokemon.Abilities;
            }
            return Page();
        }
    }
}
