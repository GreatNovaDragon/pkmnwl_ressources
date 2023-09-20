using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.encounters;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty] public Encounter Encounter { get; set; } = default!;

    public IActionResult OnGet()
    {
        return Page();
    }


    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || _context.Encounters == null || Encounter == null) return Page();

        _context.Encounters.Add(Encounter);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}