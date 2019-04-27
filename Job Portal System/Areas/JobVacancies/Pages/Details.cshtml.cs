using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;
using Job_Portal_System.Handlers;
using Job_Portal_System.Models;
using Job_Portal_System.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Areas.JobVacancies.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SignalRHub> _hubContext;

        public DetailsModel(ApplicationDbContext context,
            IHubContext<SignalRHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
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
            if (JobVacancy.Method == (int)JobVacancyMethod.Submission &&
                JobVacancy.Status == (int)JobVacancyStatus.Open &&
                User.IsInRole("JobSeeker"))
            {
                var jobSeeker = _context.JobSeekers
                    .Include(j => j.User)
                    .SingleOrDefault(j => j.User.UserName == User.Identity.Name);
                CanSubmit = jobSeeker != null && jobSeeker.IsSeeking &&
                            _context.Resumes.Any(r => r.JobSeekerId == jobSeeker.Id) &&
                            !_context.Applicants.Any(a => a.JobVacancyId == JobVacancy.Id 
                                                         && a.JobSeekerId == jobSeeker.User.Id);
            }
            else
            {
                CanSubmit = false;
            }

            return Page();
        }
        
        public async Task<IActionResult> OnPostSubmitAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            JobVacancy = await _context.JobVacancies
                .Include(j => j.User)
                .SingleOrDefaultAsync(j => j.Id == id);

            if (JobVacancy == null)
            {
                return NotFound();
            }

            if ((JobVacancyStatus)JobVacancy.Status != JobVacancyStatus.Open ||
                (JobVacancyMethod)JobVacancy.Method != JobVacancyMethod.Submission ||
                !User.IsInRole("JobSeeker"))
            {
                return BadRequest();
            }

            var jobSeeker = _context.JobSeekers
                .Include(j => j.User)
                .SingleOrDefault(j => j.User.UserName == User.Identity.Name);

            if (jobSeeker == null || !jobSeeker.IsSeeking ||
                _context.Applicants.Any(a => a.JobVacancyId == JobVacancy.Id
                                             && a.JobSeekerId == jobSeeker.User.Id))
            {
                return BadRequest();
            }

            var resume = _context.Resumes
                .SingleOrDefault(r => r.JobSeekerId == jobSeeker.Id);

            if (resume == null) return BadRequest();

            await AsyncHandler.SubmitToJobVacancy(_context, _hubContext, JobVacancy, resume);

            return RedirectToPage("./Details", new {id});
        }
    }
}
