using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Job_Portal_System.Handlers;
using Job_Portal_System.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Job_Portal_System.Areas.JobVacancies
{
    [Route("JobVacancies")]
    public class JobVacanciesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SignalRHub> _hubContext;

        public JobVacanciesController(ApplicationDbContext context,
            IHubContext<SignalRHub> hubContext)
        {
            _context = context;
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
                .Include(j => j.User)
                .Include(j => j.Applicants)
                .SingleOrDefaultAsync(j => j.Id == id);

            if (jobVacancy == null) return NotFound();
            if (jobVacancy.User.UserName != User.Identity.Name ||
                jobVacancy.Status != (int) JobVacancyStatus.Closed ||
                 jobVacancy.Applicants.Count == 0 ||
                 jobVacancy.AwaitingApplicants != 0) return BadRequest();

            // TODO: Final decision use case 

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
