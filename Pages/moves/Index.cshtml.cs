#region

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

#endregion

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
        if (_context.MoveDex != null)
            Move = await _context
                .MoveDex.Include(m => m.type)
                .Include(m => m.MoveClass)
                .ToListAsync();
    }
}
