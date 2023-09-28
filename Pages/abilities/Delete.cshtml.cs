using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.abilities;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty] public Ability Ability { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null || _context.Abilities == null) return NotFound();

        var ability = await _context.Abilities.FirstOrDefaultAsync(m => m.ID == id);

        if (ability == null)
            return NotFound();
        Ability = ability;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        if (id == null || _context.Abilities == null) return NotFound();
        var ability = await _context.Abilities.FindAsync(id);

        if (ability != null)
        {
            Ability = ability;
            _context.Abilities.Remove(Ability);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}