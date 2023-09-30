using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.pokemon;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
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
            .Include(p => p.Abilities).FirstOrDefaultAsync(m => m.ID == id);
        if (pokemon == null) return NotFound();

        var Learnset = _context.Learnsets.Include(p => p.mon).Include(m => m.move).ThenInclude(mv => mv.type)
            .Include(m => m.move).ThenInclude(mv => mv.MoveClass).Where(m => m.mon == pokemon).ToList();

        Pokemon = pokemon;
        Learnsets = Learnset;
        Abilities = Pokemon.Abilities;
        return Page();
    }
}