using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.items;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty] public Item Item { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null || _context.Items == null) return NotFound();

        var item = await _context.Items.FirstOrDefaultAsync(m => m.ID == id);

        if (item == null)
            return NotFound();
        Item = item;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        if (id == null || _context.Items == null) return NotFound();
        var item = await _context.Items.FindAsync(id);

        if (item != null)
        {
            Item = item;
            _context.Items.Remove(Item);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}