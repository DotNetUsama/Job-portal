using System;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;
using Job_Portal_System.Models;
using Job_Portal_System.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Controllers
{
    [Route("Applicants")]
    public class ApplicantsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<SignalRHub> _hubContext;

        public ApplicantsController(ApplicationDbContext context,
            UserManager<User> userManager,
            IHubContext<SignalRHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Details(string id)
        {
            if (!User.IsInRole("Recruiter") && !User.IsInRole("JobSeeker")) return BadRequest();

            Applicant applicant;


            if (User.IsInRole("JobSeeker"))
            {
                applicant = await _context.Applicants
                    .Include(a => a.JobSeeker)
                    .SingleOrDefaultAsync(a => a.Id == id);

                if (applicant == null) return NotFound();
                if (applicant.JobSeeker.UserName != User.Identity.Name) return BadRequest();

                applicant.JobVacancy = await _context.JobVacancies
                    .Include(j => j.CompanyDepartment).ThenInclude(d => d.Company)
                    .Include(j => j.CompanyDepartment).ThenInclude(d => d.City)
                    .Include(r => r.EducationQualifications).ThenInclude(e => e.FieldOfStudy)
                    .Include(r => r.WorkExperienceQualifications).ThenInclude(w => w.JobTitle)
                    .Include(r => r.DesiredSkills).ThenInclude(s => s.Skill)
                    .Include(r => r.JobTypes)
                    .Include(r => r.JobTitle)
                    .SingleOrDefaultAsync(j => j.Id == applicant.JobVacancyId);
                return View("DetailsForJobSeeker", applicant);
            }

            applicant = await _context.Applicants
                .Include(a => a.Recruiter)
                .SingleOrDefaultAsync(a => a.Id == id);

            if (applicant == null) return NotFound();
            if (applicant.Recruiter.UserName != User.Identity.Name) return BadRequest();

            applicant.Resume = await _context.Resumes
                .Include(r => r.Educations).ThenInclude(e => e.FieldOfStudy)
                .Include(r => r.Educations).ThenInclude(e => e.School).ThenInclude(s => s.City)
                .Include(r => r.WorkExperiences).ThenInclude(w => w.JobTitle)
                .Include(r => r.WorkExperiences).ThenInclude(w => w.Company)
                .Include(r => r.OwnedSkills).ThenInclude(s => s.Skill)
                .Include(r => r.User)
                .SingleOrDefaultAsync(r => r.Id == applicant.ResumeId);
            applicant.JobVacancy = await _context.JobVacancies
                .SingleOrDefaultAsync(j => j.Id == applicant.JobVacancyId);
            return View("DetailsForRecruiter", applicant);
        }
        
        [HttpPost]
        [Route("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus(string id, int status)
        {
            var applicant = await _context.Applicants
                .SingleOrDefaultAsync(a => a.Id == id);

            if (applicant == null) return NotFound();
            if (User == null) return Unauthorized();

            var userType = User.IsInRole("JobSeeker") ? UserType.JobSeeker :
                User.IsInRole("Recruiter") ? UserType.Recruiter : UserType.Other;

            if (!((ApplicantStatus) applicant.Status).IsNextState((ApplicantStatus)status, userType))
                return BadRequest();

            await ProcessChangingStatus(applicant, status);

            return Redirect(Request.Headers["Referer"].ToString());
        }

        private async Task ProcessChangingStatus(Applicant applicant, int newStatus)
        {
            JobVacancy jobVacancy;
            switch ((ApplicantStatus)newStatus)
            {
                case ApplicantStatus.DummyAccepted:
                case ApplicantStatus.DummyRejected:
                    if (applicant.Status != (int) ApplicantStatus.WaitingRecruiterDecision) return;
                    jobVacancy = await _context.JobVacancies
                        .SingleOrDefaultAsync(j => j.Id == applicant.JobVacancyId);
                    jobVacancy.AwaitingApplicants--;
                    return;
                case ApplicantStatus.WaitingRecruiterDecision:
                case ApplicantStatus.AcceptMeeting:
                case ApplicantStatus.RejectMeeting:
                    var recruiter = await _userManager.FindByIdAsync(applicant.RecruiterId);
                    jobVacancy = await _context.JobVacancies
                        .SingleOrDefaultAsync(j => j.Id == applicant.JobVacancyId);
                    await _context.SendNotificationAsync(_hubContext, new Notification
                    {
                        Type = (int)((ApplicantStatus)newStatus).GetNotificationType(),
                        EntityId = applicant.Id,
                        Peer1 = recruiter.FirstName,
                        Peer2 = jobVacancy.Title,
                    }, recruiter);
                    return;

                case ApplicantStatus.RejectedRecommendation:
                case ApplicantStatus.AcceptedByRecruiter:
                case ApplicantStatus.RejectedByRecruiter:
                    break;
                case ApplicantStatus.PendingRecommendation:
                    return;
                default:
                    return;
            }
            applicant.Status = newStatus;
            await _context.SaveChangesAsync();
        }
    }
}

