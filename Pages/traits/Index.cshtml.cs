#region

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

#endregion

namespace pkmnWildLife.Pages.traits;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Ability> Ability { get; set; } = default!;

    public async Task OnGetAsync()
    {
        if (_context.AbilityDex != null)
            Ability = await _context.AbilityDex.Where(m => m.IsTrait).ToListAsync();
    }
}
