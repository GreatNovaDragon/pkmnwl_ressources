using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.LearnSet;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Learnset> Learnset { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Learnset = await _context.Learnsets.ToListAsync();
    }
}
