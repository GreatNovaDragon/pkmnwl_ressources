using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.encounters;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Encounter> Encounter { get; set; } = default!;

    public async Task OnGetAsync()
    {
        if (_context.Encounters != null) Encounter = await _context.Encounters.ToListAsync();
    }
}