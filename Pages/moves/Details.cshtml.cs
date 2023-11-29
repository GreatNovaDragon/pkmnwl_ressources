#region

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

#endregion

namespace pkmnWildLife.Pages.moves;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Move Move { get; set; } = default!;

    public IList<Learnset> LearnedBy { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null || _context.MoveDex == null) return NotFound();

        var move = await _context.MoveDex.Include(m => m.type).Include(m => m.MoveClass)
            .FirstOrDefaultAsync(m => m.ID == id);
        if (move == null)
            return NotFound();
        Move = move;

        if (_context.Learnsets != null)
            LearnedBy = await _context.Learnsets.Where(m => m.move == Move).Include(m => m.mon)
                .ThenInclude(m => m.Type1).Include(m => m.mon).ThenInclude(m => m.Type2).ToListAsync();

        return Page();
    }
}