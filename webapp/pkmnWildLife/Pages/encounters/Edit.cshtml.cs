using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.encounters;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty] public Encounter Encounter { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null || _context.Encounters == null) return NotFound();

        var encounter = await _context.Encounters.FirstOrDefaultAsync(m => m.ID == id);
        if (encounter == null) return NotFound();
        Encounter = encounter;
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        _context.Attach(Encounter).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EncounterExists(Encounter.ID))
                return NotFound();
            throw;
        }

        return RedirectToPage("./Index");
    }

    private bool EncounterExists(string id)
    {
        return (_context.Encounters?.Any(e => e.ID == id)).GetValueOrDefault();
    }
}