#region

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

#endregion

namespace pkmnWildLife.Pages.pokemon;

public class PrintModel : PageModel
{
    public readonly ApplicationDbContext _context;

    public PrintModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Pokemon Pokemon { get; set; } = default!;
    public List<Learnset> Learnsets { get; set; }

    public List<Ability> Abilities { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null || _context.Pokedex == null) return NotFound();

        var pokemon = await _context.Pokedex
            .Include(p => p.Abilities).Include(p => p.Type1).Include(p => p.Type2).FirstOrDefaultAsync(m => m.ID == id);
        if (pokemon == null) return NotFound();

        var Learnset = _context.Learnsets.Include(p => p.mon).Include(m => m.move).ThenInclude(mv => mv.type)
            .Include(m => m.move).ThenInclude(mv => mv.MoveClass).Where(m => m.mon == pokemon).ToList();

        // if (pokemon.ID == "mew") Learnset.RemoveAll(m => m.how != "level-up");

        Pokemon = pokemon;
        Learnsets = Learnset;
        Abilities = Pokemon.Abilities;
        return Page();
    }
}