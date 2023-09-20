using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.characters;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Character> Character { get; set; } = default!;

    public async Task OnGetAsync()
    {
        if (_context.Characters != null) Character = await _context.Characters.ToListAsync();
    }
}