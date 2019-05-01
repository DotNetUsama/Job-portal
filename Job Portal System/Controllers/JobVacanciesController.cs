using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;
using Job_Portal_System.Handlers;
using Job_Portal_System.Models;
using Job_Portal_System.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Controllers
{
    [Route("JobVacancies")]
    public class JobVacanciesController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<SignalRHub> _hubContext;

        public JobVacanciesController(IHostingEnvironment env,
            ApplicationDbContext context,
            UserManager<User> userManager,
            IHubContext<SignalRHub> hubContext)
        {
            _env = env;
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        [HttpPost]
        [Route("Close")]
        public async Task<IActionResult> Close(string id)
        {

            if (id == null) return NotFound();

            var jobVacancy = await _context.JobVacancies
                .Include(j => j.User)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (jobVacancy == null) return NotFound();

            if (jobVacancy.User.UserName != User.Identity.Name ||
                jobVacancy.Status != (int)JobVacancyStatus.Open ||
                jobVacancy.Method != (int)JobVacancyMethod.Submission)
            {
                return BadRequest();
            }

            jobVacancy.Status = (int)JobVacancyStatus.Closed;

            await _context.SaveChangesAsync();

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [Route("Submit")]
        public async Task<IActionResult> Submit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobVacancy = await _context.JobVacancies
                .Include(j => j.User)
                .SingleOrDefaultAsync(j => j.Id == id);

            if (jobVacancy == null)
            {
                return NotFound();
            }

            if ((JobVacancyStatus)jobVacancy.Status != JobVacancyStatus.Open ||
                (JobVacancyMethod)jobVacancy.Method != JobVacancyMethod.Submission ||
                !User.IsInRole("JobSeeker"))
            {
                return BadRequest();
            }

            var jobSeeker = await _context.JobSeekers
                .Include(j => j.User)
                .SingleOrDefaultAsync(j => j.User.UserName == User.Identity.Name);

            if (jobSeeker == null || !jobSeeker.IsSeeking ||
                _context.Applicants.Any(a => a.JobVacancyId == jobVacancy.Id
                                             && a.JobSeekerId == jobSeeker.User.Id))
            {
                return BadRequest();
            }

            var resume = _context.Resumes
                .SingleOrDefault(r => r.JobSeekerId == jobSeeker.Id);

            if (resume == null) return BadRequest();

            await AsyncHandler.SubmitToJobVacancy(_context, _hubContext, jobVacancy, resume);

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [Route("FinalDecision")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> FinalDecision(string id)
        {
            var jobVacancy = await _context.JobVacancies
                .SingleOrDefaultAsync(j => j.Id == id);

            if (jobVacancy == null) return NotFound();
            if (jobVacancy.Status != (int) JobVacancyStatus.Closed ||
                jobVacancy.AwaitingApplicants != 0) return BadRequest();

            var jobVacancyOwner = await _userManager.FindByIdAsync(jobVacancy.UserId);
            if (jobVacancyOwner.UserName != User.Identity.Name) return BadRequest();

            var applicantsCount = _context.Applicants
                .Count(a => a.JobVacancyId == jobVacancy.Id);
            if (applicantsCount == 0) return BadRequest();

            jobVacancy.EducationQualifications = _context.EducationQualifications
                .Where(e => e.JobVacancyId == jobVacancy.Id)
                .ToList();
            jobVacancy.WorkExperienceQualifications = _context.WorkExperienceQualifications
                .Where(w => w.JobVacancyId == jobVacancy.Id)
                .ToList();
            jobVacancy.DesiredSkills = _context.DesiredSkills
                .Where(s => s.JobVacancyId == jobVacancy.Id)
                .ToList();

            await AsyncHandler.FinalDecideOnApplicants(_env, _context, _hubContext, jobVacancy);

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
