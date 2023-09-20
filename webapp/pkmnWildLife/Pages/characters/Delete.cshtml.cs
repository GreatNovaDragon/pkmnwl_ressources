using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.characters;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty] public Character Character { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null || _context.Characters == null) return NotFound();

        var character = await _context.Characters.FirstOrDefaultAsync(m => m.ID == id);

        if (character == null)
            return NotFound();
        Character = character;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        if (id == null || _context.Characters == null) return NotFound();
        var character = await _context.Characters.FindAsync(id);

        if (character != null)
        {
            Character = character;
            _context.Characters.Remove(Character);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}