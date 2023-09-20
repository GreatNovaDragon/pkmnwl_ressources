using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.damagedice;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty] public DamageDice DamageDice { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null || _context.DamageDice == null) return NotFound();

        var damagedice = await _context.DamageDice.FirstOrDefaultAsync(m => m.ID == id);
        if (damagedice == null) return NotFound();
        DamageDice = damagedice;
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        _context.Attach(DamageDice).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DamageDiceExists(DamageDice.ID))
                return NotFound();
            throw;
        }

        return RedirectToPage("./Index");
    }

    private bool DamageDiceExists(string id)
    {
        return (_context.DamageDice?.Any(e => e.ID == id)).GetValueOrDefault();
    }
}