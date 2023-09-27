using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.moves;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Move> Move { get; set; } = default!;

    public async Task OnGetAsync()
    {
        if (_context.Moves != null)
            Move = await _context.Moves.Include(m => m.type).Include(m => m.MoveClass).ToListAsync();
    }
}