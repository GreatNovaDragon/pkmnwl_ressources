#region

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;
using Type = pkmnWildLife.Data.Type;

#endregion

namespace pkmnWildLife.Pages.pokemon;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Pokemon Pokemon { get; set; } = default!;
    public List<Learnset> Learnsets { get; set; }

    public List<Ability> Abilities { get; set; }

    public List<(Type, int)> DamageMultiplier { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null || _context.Pokedex == null)
            return NotFound();

        var pokemon = await _context
            .Pokedex.Include(p => p.Abilities)
            .Include(p => p.Type1)
            .Include(p => p.Type2)
            .FirstOrDefaultAsync(m => m.ID == id);
        if (pokemon == null)
            return NotFound();

        var Learnset = _context
            .Learnsets.Include(p => p.mon)
            .Include(m => m.move)
            .ThenInclude(mv => mv.type)
            .Include(m => m.move)
            .ThenInclude(mv => mv.MoveClass)
            .Where(m => m.mon == pokemon)
            .ToList();

        Pokemon = pokemon;
        Learnsets = Learnset;
        Abilities = Pokemon.Abilities;
        DamageMultiplier = GetWeaknesses();
        return Page();
    }

    public List<(Type, int)> GetWeaknesses()
    {
        List<(Type, int)> returnlist = new();
        var type_1 = _context
            .DamageRelations.Include(damageRelations => damageRelations.noDamageFrom)
            .FirstOrDefault(e => e.Type == Pokemon.Type1);
        if (Pokemon.Type2 == null)
        {
            foreach (var v in type_1.doubleDamageFrom)
                returnlist.Add((v, 200));

            foreach (var v in type_1.noDamageFrom)
                returnlist.Add((v, 0));

            foreach (var v in type_1.halfDamageFrom)
                returnlist.Add((v, 50));
        }
        else
        {
            var type_2 = _context.DamageRelations.FirstOrDefault(e => e.Type == Pokemon.Type2);
            foreach (var t in _context.Types)
            {
                var mult = 100;
                if (type_1.noDamageFrom.Contains(t))
                    mult *= 0;
                if (type_2.noDamageFrom.Contains(t))
                    mult *= 0;
                if (type_1.halfDamageFrom.Contains(t))
                    mult /= 2;
                if (type_2.halfDamageFrom.Contains(t))
                    mult /= 2;
                if (type_1.doubleDamageFrom.Contains(t))
                    mult *= 2;
                if (type_2.doubleDamageFrom.Contains(t))
                    mult *= 2;
                returnlist.Add((t, mult));
            }
        }

        return returnlist;
    }
}
