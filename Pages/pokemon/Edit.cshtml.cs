using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.pokemon;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty] public Pokemon Pokemon { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null || _context.Pokedex == null) return NotFound();

        var pokemon = await _context.Pokedex.FirstOrDefaultAsync(m => m.ID == id);
        if (pokemon == null) return NotFound();
        Pokemon = pokemon;
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        _context.Attach(Pokemon).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PokemonExists(Pokemon.ID))
                return NotFound();
            throw;
        }

        return RedirectToPage("./Index");
    }

    private bool PokemonExists(string id)
    {
        return (_context.Pokedex?.Any(e => e.ID == id)).GetValueOrDefault();
    }
}