using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.skills;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty] public Skill Skill { get; set; } = default!;

    public IActionResult OnGet()
    {
        return Page();
    }


    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || _context.Skills == null || Skill == null) return Page();

        _context.Skills.Add(Skill);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}