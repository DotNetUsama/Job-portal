using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;
using Job_Portal_System.Handlers;
using Job_Portal_System.Models;
using Job_Portal_System.SignalR;
using Job_Portal_System.Utilities.Semantic;
using Job_Portal_System.ViewModels;
using JW;
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
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> Close(string id)
        {
            if (id == null) return NotFound();

            var jobVacancy = await _context.JobVacancies
                .Include(j => j.User)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (jobVacancy == null) return NotFound();

            if (jobVacancy.User.UserName != User.Identity.Name ||
                jobVacancy.Status != (int)JobVacancyStatus.Open)
            {
                return BadRequest();
            }

            jobVacancy.Status = (int)JobVacancyStatus.Closed;

            if (jobVacancy.Method == (int) JobVacancyMethod.Recommendation)
            {
                var pendingApplicants = _context.Applicants
                    .Where(a => a.JobVacancyId == jobVacancy.Id &&
                                a.Status == (int) ApplicantStatus.PendingRecommendation);
                foreach (var pendingApplicant in pendingApplicants)
                {
                    pendingApplicant.Status = (int) ApplicantStatus.RejectedByRecruiter;
                }
            }

            await _context.SaveChangesAsync();

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [Route("Delete")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var jobVacancy = await _context.JobVacancies
                .Include(j => j.User)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (jobVacancy == null) return NotFound();

            if (jobVacancy.User.UserName != User.Identity.Name) return BadRequest();

            if (jobVacancy.Method != (int) JobVacancyMethod.Submission &&
                (jobVacancy.Method != (int) JobVacancyMethod.Recommendation ||
                 jobVacancy.Status == (int) JobVacancyStatus.Open)) return BadRequest();

            await AsyncHandler.DeleteJobVacancy(_context, _hubContext, jobVacancy);
            
            return LocalRedirect("/JobVacancies");
        }
        
        [HttpPost]
        [Route("Submit")]
        [Authorize(Roles = "JobSeeker")]
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

            if (jobSeeker == null) return BadRequest();

            var resume = _context.Resumes
                .SingleOrDefault(r => r.JobSeekerId == jobSeeker.Id);
            
            if (resume == null || !resume.IsSeeking ||
                _context.Applicants.Any(a => a.JobVacancyId == jobVacancy.Id
                                             && a.JobSeekerId == jobSeeker.User.Id))
            {
                return BadRequest();
            }

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

        [HttpGet]
        [Route("Search")]
        public IActionResult Search(string query, int p = 1)
        {
            query = query.ToLower().Trim();
            var similarities = SimilaritiesOperator.GetSimilarities(query, _env);
            var jobTitles = similarities
                .Select(similarity => _context.JobTitles.SingleOrDefault(j => j.NormalizedTitle == similarity))
                .Where(jobTitle => jobTitle != null)
                .ToList();

            var jobVacancies = new List<JobVacancy>();
            foreach (var jobTitle in jobTitles)
            {
                jobVacancies.AddRange(_context.JobVacancies
                    .Include(j => j.JobTitle)
                    .Include(j => j.CompanyDepartment).ThenInclude(c => c.Company)
                    .Where(r => r.JobTitleId == jobTitle.Id));
            }

            var pager = new Pager(jobVacancies.Count, p);
            
            var viewedJobVacancies = jobVacancies
                .Distinct()
                .Skip((pager.CurrentPage - 1) * pager.PageSize)
                .Take(pager.PageSize);

            return View("JobVacanciesSearchResult", new JobVacanciesSearchResult
            {
                JobVacancies = viewedJobVacancies,
                Query = query,
                TotalPages = pager.TotalPages,
                PageNumber = pager.CurrentPage,
                IsFirst = pager.CurrentPage == 1 || pager.CurrentPage == 0,
                IsLast = pager.CurrentPage == pager.TotalPages,
            });
        }
    }
}
