#region

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

#endregion

namespace pkmnWildLife.Pages.pokemon;

public class IndexModel(ApplicationDbContext context) : PageModel
{
    public IList<Pokemon> Pokemon { get; set; } = default!;

    public async Task OnGetAsync()
    {
        if (context.Pokedex != null)
            Pokemon = await context.Pokedex.Include(c => c.Type1).Include(c => c.Type2).ToListAsync();
    }
}