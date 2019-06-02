using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Areas.Resumes.Pages.Download
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Resume Resume { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Resume = await _context.Resumes
                .Include(r => r.Educations).ThenInclude(e => e.FieldOfStudy)
                .Include(r => r.Educations).ThenInclude(e => e.School)
                .Include(r => r.WorkExperiences).ThenInclude(w => w.JobTitle)
                .Include(r => r.WorkExperiences).ThenInclude(w => w.Company)
                .Include(r => r.OwnedSkills).ThenInclude(s => s.Skill)
                .Include(r => r.User)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (Resume == null)
            {
                return NotFound();
            }

            if (Resume.IsPublic) return Page();

            return BadRequest();
        }
    }
}
