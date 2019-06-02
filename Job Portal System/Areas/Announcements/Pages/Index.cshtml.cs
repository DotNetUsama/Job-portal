using System.Collections.Generic;
using System.Linq;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Job_Portal_System.Areas.Announcements.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool IsAdmin { get; set; }

        public IEnumerable<Announcement> Announcements { get; set; }

        public IActionResult OnGet(string jobVacancyId)
        {
            IsAdmin = User.IsInRole("Administrator");
            Announcements = _context.Announcements.OrderByDescending(a => a.CreatedAt);
            return Page();
        }
    }
}