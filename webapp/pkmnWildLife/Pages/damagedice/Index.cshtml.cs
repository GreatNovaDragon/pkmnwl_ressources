using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.damagedice;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<DamageDice> DamageDice { get; set; } = default!;

    public async Task OnGetAsync()
    {
        if (_context.DamageDice != null) DamageDice = await _context.DamageDice.ToListAsync();
    }
}