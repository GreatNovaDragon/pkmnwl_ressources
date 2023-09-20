using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.attackModifiers;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<AttackMod> AttackMod { get; set; } = default!;

    public async Task OnGetAsync()
    {
        if (_context.AttackModifiers != null) AttackMod = await _context.AttackModifiers.ToListAsync();
    }
}