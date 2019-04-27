using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Areas.Applicants.Pages
{
    [Authorize(Roles = "Recruiter")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public JobVacancy JobVacancy { get; set; }

        public async Task<IActionResult> OnGetAsync(string jobVacancyId)
        {
            JobVacancy = await _context.JobVacancies
                .Include(j => j.Applicants).ThenInclude(a => a.JobSeeker)
                .Include(j => j.CompanyDepartment).ThenInclude(d => d.City)
                .Include(j => j.JobTitle)
                .Include(j => j.User)
                .SingleOrDefaultAsync(j => j.Id == jobVacancyId);

            if (JobVacancy == null) return NotFound();
            if (JobVacancy.User.UserName != User.Identity.Name) return BadRequest();

            return Page();
        }
    }
}