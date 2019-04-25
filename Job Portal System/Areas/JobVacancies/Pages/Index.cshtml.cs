using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Areas.JobVacancies.Pages
{
    [Authorize(Roles = "Recruiter")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<JobVacancy> JobVacancies { get; set; }

        public async Task OnGetAsync()
        {
            JobVacancies = await _context.JobVacancies
                .Include(j => j.CompanyDepartment).ThenInclude(d => d.City)
                .Include(j => j.JobTitle)
                .Where(j => j.User.UserName == User.Identity.Name)
                .ToListAsync();
        }
    }
}