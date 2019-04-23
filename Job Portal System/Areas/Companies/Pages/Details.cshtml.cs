using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Areas.Companies.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Company Company { get; set; }
        public bool CanEdit { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Company = await _context.Companies.FirstOrDefaultAsync(company => company.Id == id);

            if (Company == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Administrator"))
            {
                CanEdit = true;
            }
            else
            {
                var recruiterInDb = await _context.Recruiters
                    .SingleOrDefaultAsync(recruiter => recruiter.User.UserName == User.Identity.Name);
                if (recruiterInDb == null || Company.Id != recruiterInDb.CompanyId)
                {
                    CanEdit = false;
                }
                else
                {
                    CanEdit = true;
                }
            }

            return Page();
        }
    }
}
