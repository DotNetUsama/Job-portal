using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Areas.JobVacancies.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public JobVacancy JobVacancy { get; set; }
        public bool IsOwner { get; set; }
        public bool CanSubmit { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            JobVacancy = await _context.JobVacancies
                .Include(j => j.CompanyDepartment).ThenInclude(d => d.Company)
                .Include(j => j.CompanyDepartment).ThenInclude(d => d.City)
                .Include(r => r.EducationQualifications).ThenInclude(e => e.FieldOfStudy)
                .Include(r => r.WorkExperienceQualifications).ThenInclude(w => w.JobTitle)
                .Include(r => r.DesiredSkills).ThenInclude(s => s.Skill)
                .Include(r => r.JobTypes)
                .Include(r => r.JobTitle)
                .Include(r => r.User)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (JobVacancy == null)
            {
                return NotFound();
            }

            IsOwner = JobVacancy.User.UserName == User.Identity.Name;
            CanSubmit = (JobVacancyMethod) JobVacancy.Method == JobVacancyMethod.Submission &&
                        (JobVacancyStatus) JobVacancy.Status == JobVacancyStatus.Open &&
                        User.IsInRole("JobSeeker");
            return Page();
        }

        public async Task<IActionResult> OnPostCloseAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            JobVacancy = await _context.JobVacancies
                .Include(j => j.User)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (JobVacancy == null)
            {
                return NotFound();
            }

            if (JobVacancy.User.UserName != User.Identity.Name ||
                (JobVacancyStatus) JobVacancy.Status != JobVacancyStatus.Open ||
                (JobVacancyMethod) JobVacancy.Method != JobVacancyMethod.Submission)
            {
                return BadRequest();
            }

            JobVacancy.Status = (int)JobVacancyStatus.Closed;

            //TODO: CLOSING JOB VACANCY USE CASE
            return Page();
        }
        
        public async Task<IActionResult> OnPostSubmitAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            JobVacancy = await _context.JobVacancies.SingleOrDefaultAsync(m => m.Id == id);

            if (JobVacancy == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("JobSeeker") ||
                (JobVacancyStatus)JobVacancy.Status != JobVacancyStatus.Open ||
                (JobVacancyMethod)JobVacancy.Method != JobVacancyMethod.Submission)
            {
                return BadRequest();
            }
            
            //TODO: SUBMISSION USE CASE
            return Page();
        }
    }
}
