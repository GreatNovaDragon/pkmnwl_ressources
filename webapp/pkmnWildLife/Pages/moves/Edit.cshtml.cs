using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.moves;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty] public Move Move { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null || _context.Moves == null) return NotFound();

        var move = await _context.Moves.FirstOrDefaultAsync(m => m.ID == id);
        if (move == null) return NotFound();
        Move = move;
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        _context.Attach(Move).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MoveExists(Move.ID))
                return NotFound();
            throw;
        }

        return RedirectToPage("./Index");
    }

    private bool MoveExists(string id)
    {
        return (_context.Moves?.Any(e => e.ID == id)).GetValueOrDefault();
    }
}