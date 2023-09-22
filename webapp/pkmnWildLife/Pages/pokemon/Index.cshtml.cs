using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.pokemon;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Pokemon> Pokemon { get; set; } = default!;

    public async Task OnGetAsync()
    {
        if (_context.Pokedex != null) Pokemon = await _context.Pokedex.ToListAsync();
    }
}