using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using pkmnWildLife.Data;

namespace pkmnWildLife.Pages.moves
{
    public class DetailsModel : PageModel
    {
        private readonly pkmnWildLife.Data.ApplicationDbContext _context;

        public DetailsModel(pkmnWildLife.Data.ApplicationDbContext context)
        {
            _context = context;
        }

      public Move Move { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.Moves == null)
            {
                return NotFound();
            }

            var move = await _context.Moves.FirstOrDefaultAsync(m => m.ID == id);
            if (move == null)
            {
                return NotFound();
            }
            else 
            {
                Move = move;
            }
            return Page();
        }
    }
}