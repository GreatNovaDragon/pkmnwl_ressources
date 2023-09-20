using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.pokemon
{
    public class CreateModel : PageModel
    {
        private readonly pkmnWildLife.Data.ApplicationDbContext _context;

        public CreateModel(pkmnWildLife.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Pokemon Pokemon { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Pokedex == null || Pokemon == null)
            {
                return Page();
            }

            _context.Pokedex.Add(Pokemon);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
