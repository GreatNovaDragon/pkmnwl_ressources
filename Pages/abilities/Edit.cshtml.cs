using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.abilities;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty] public Ability Ability { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null || _context.Abilities == null) return NotFound();

        var ability = await _context.Abilities.FirstOrDefaultAsync(m => m.ID == id);
        if (ability == null) return NotFound();
        Ability = ability;
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        _context.Attach(Ability).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AbilityExists(Ability.ID))
                return NotFound();
            throw;
        }

        return RedirectToPage("./Index");
    }

    private bool AbilityExists(string id)
    {
        return (_context.Abilities?.Any(e => e.ID == id)).GetValueOrDefault();
    }
}