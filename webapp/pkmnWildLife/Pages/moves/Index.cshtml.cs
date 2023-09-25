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
    public class IndexModel : PageModel
    {
        private readonly pkmnWildLife.Data.ApplicationDbContext _context;

        public IndexModel(pkmnWildLife.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Move> Move { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Moves != null)
            {
                Move = await _context.Moves.ToListAsync();
            }
        }
    }
}
