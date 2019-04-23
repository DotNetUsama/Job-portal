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
    public class IndexModel : PageModel
    {
        private readonly Job_Portal_System.Data.ApplicationDbContext _context;

        public IndexModel(Job_Portal_System.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Company> Company { get;set; }

        public async Task OnGetAsync()
        {
            Company = await _context.Companies.ToListAsync();
        }
    }
}
