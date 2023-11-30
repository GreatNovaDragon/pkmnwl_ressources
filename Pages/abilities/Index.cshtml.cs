#region

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

#endregion

namespace pkmnWildLife.Pages.abilities;

public class IndexModel(ApplicationDbContext context) : PageModel
{
    public IList<Ability> Ability { get; set; } = default!;

    public async Task OnGetAsync()
    {
        if (context.AbilityDex != null) Ability = await context.AbilityDex.Where(a => !a.IsTrait).ToListAsync();
    }
}