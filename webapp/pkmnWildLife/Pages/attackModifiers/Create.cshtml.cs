using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.attackModifiers;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty] public AttackMod AttackMod { get; set; } = default!;

    public IActionResult OnGet()
    {
        return Page();
    }


    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || _context.AttackModifiers == null || AttackMod == null) return Page();

        _context.AttackModifiers.Add(AttackMod);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}