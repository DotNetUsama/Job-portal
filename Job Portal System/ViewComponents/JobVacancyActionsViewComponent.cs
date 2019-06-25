using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;
using Job_Portal_System.Models;
using Job_Portal_System.ViewModels.JobVacancies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.ViewComponents
{
    public class JobVacancyActionsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public JobVacancyActionsViewComponent(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string jobVacancyId, ClaimsPrincipal user, bool side = false)
        {
            var userId = _userManager.GetUserId(user);
            var jobVacancy = _context.JobVacancies.FirstOrDefault(j => j.Id == jobVacancyId);

            if (jobVacancy == null) return View("ForJobSeeker", new JobVacancyActionsForJobSeekerViewModel());

            var isOwner = jobVacancy.UserId == userId;

            if (isOwner)
            {
                var jobVacancyActions = new JobVacancyActionsForRecruiterViewModel
                {
                    JobVacancyId = jobVacancy.Id,
                    CanClose = jobVacancy.Status == (int)JobVacancyStatus.Open &&
                               await _context.Applicants
                                   .CountAsync(a => 
                                       a.JobVacancyId == jobVacancy.Id &&
                                       (jobVacancy.Method != (int)JobVacancyMethod.Recommendation ||
                                        a.Status == (int)ApplicantStatus.WaitingRecruiterDecision)) != 0,
                    CanDelete = jobVacancy.Method == (int)JobVacancyMethod.Submission ||
                                jobVacancy.Method == (int)JobVacancyMethod.Recommendation &&
                                jobVacancy.Status != (int)JobVacancyStatus.Open,
                    CanFinalDecide = jobVacancy.Status == (int)JobVacancyStatus.Closed
                                     && jobVacancy.AwaitingApplicants == 0
                                     && jobVacancy.Applicants.Count != 0
                };
                return View(side ? "ForRecruiterSide" : "ForRecruiter", jobVacancyActions);
            }
            else
            {
                var jobVacancyActions = new JobVacancyActionsForJobSeekerViewModel
                {
                    JobVacancyId = jobVacancy.Id,
                    CanSubmit = false,
                };

                if (jobVacancy.Method != (int) JobVacancyMethod.Submission ||
                    jobVacancy.Status != (int) JobVacancyStatus.Open ||
                    !User.IsInRole("JobSeeker")) return View("ForJobSeeker", jobVacancyActions);

                var jobSeeker = await _context.JobSeekers
                    .Include(j => j.User)
                    .FirstOrDefaultAsync(j => j.UserId == userId);

                if (jobSeeker == null) return View("ForJobSeeker", jobVacancyActions);

                var resume = await _context.Resumes
                    .FirstOrDefaultAsync(r => r.JobSeekerId == jobSeeker.Id);
                jobVacancyActions.CanSubmit = 
                    resume != null && 
                    resume.IsSeeking &&
                    !_context.Applicants.Any(a => 
                        a.JobVacancyId == jobVacancy.Id && 
                        a.ResumeId == resume.Id);

                return View("ForJobSeeker", jobVacancyActions);
            }
        }
    }
}
