using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.damagedice;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public DamageDice DamageDice { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null || _context.DamageDice == null) return NotFound();

        var damagedice = await _context.DamageDice.FirstOrDefaultAsync(m => m.ID == id);
        if (damagedice == null)
            return NotFound();
        DamageDice = damagedice;
        return Page();
    }
}