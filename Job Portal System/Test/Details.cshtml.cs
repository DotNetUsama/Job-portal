using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Job_Portal_System.Data;
using Job_Portal_System.Models;

namespace Job_Portal_System.Test
{
    public class DetailsModel : PageModel
    {
        private readonly Job_Portal_System.Data.ApplicationDbContext _context;

        public DetailsModel(Job_Portal_System.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Company Company { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Company = await _context.Companies.FirstOrDefaultAsync(m => m.Id == id);

            if (Company == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
