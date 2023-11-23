#region

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

#endregion

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
        if (_context.Pokedex != null)
            Pokemon = await _context.Pokedex.Include(c => c.Type1).Include(c => c.Type2).ToListAsync();
    }
}