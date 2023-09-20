using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.skills;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty] public Skill Skill { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null || _context.Skills == null) return NotFound();

        var skill = await _context.Skills.FirstOrDefaultAsync(m => m.ID == id);

        if (skill == null)
            return NotFound();
        Skill = skill;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        if (id == null || _context.Skills == null) return NotFound();
        var skill = await _context.Skills.FindAsync(id);

        if (skill != null)
        {
            Skill = skill;
            _context.Skills.Remove(Skill);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}