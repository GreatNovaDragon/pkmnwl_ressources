using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.moves;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty] public Move Move { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null || _context.Moves == null) return NotFound();

        var move = await _context.Moves.FirstOrDefaultAsync(m => m.ID == id);

        if (move == null)
            return NotFound();
        Move = move;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        if (id == null || _context.Moves == null) return NotFound();
        var move = await _context.Moves.FindAsync(id);

        if (move != null)
        {
            Move = move;
            _context.Moves.Remove(Move);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}