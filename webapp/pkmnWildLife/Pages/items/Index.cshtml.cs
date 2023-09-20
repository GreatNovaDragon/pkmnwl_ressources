using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.items;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Item> Item { get; set; } = default!;

    public async Task OnGetAsync()
    {
        if (_context.Items != null) Item = await _context.Items.ToListAsync();
    }
}