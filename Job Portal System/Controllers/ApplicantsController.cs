using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;
using Job_Portal_System.Handlers;
using Job_Portal_System.Managers;
using Job_Portal_System.Models;
using Job_Portal_System.Utilities.RankingSystem;
using Job_Portal_System.SignalR;
using Job_Portal_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Controllers
{
    [Route("Applicants")]
    public class ApplicantsController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<SignalRHub> _hubContext;

        public ApplicantsController(IHostingEnvironment env,
            ApplicationDbContext context,
            UserManager<User> userManager,
            IHubContext<SignalRHub> hubContext)
        {
            _env = env;
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
                    .Include(r => r.JobTypes)
                    .Include(r => r.JobTitle)
                    .SingleOrDefaultAsync(j => j.Id == applicant.JobVacancyId);

                EvaluatedJobSeekerApplicant viewModel;
                try
                {
                    var evaluation = FilesManager.Read<Evaluation>(_env, "Evaluations", applicant.Id);
                    viewModel = new EvaluatedJobSeekerApplicant
                    {
                        Applicant = applicant,
                        IsEvaluated = true,
                        Educations = _context.EducationQualifications
                            .Include(e => e.FieldOfStudy)
                            .Where(e => e.JobVacancyId == applicant.JobVacancyId)
                            .Select(e => new EvaluatedEducationQualification(e,
                                evaluation.EducationsRanks[e.FieldOfStudyId]))
                            .ToList(),
                        WorkExperiences = _context.WorkExperienceQualifications
                            .Include(w => w.JobTitle)
                            .Where(w => w.JobVacancyId == applicant.JobVacancyId)
                            .Select(w => new EvaluatedWorkExperienceQualification(w,
                                evaluation.WorkExperiencesRanks[w.JobTitleId]))
                            .ToList(),
                        DesiredSkills = _context.DesiredSkills
                            .Include(s => s.Skill)
                            .Where(s => s.JobVacancyId == applicant.JobVacancyId)
                            .Select(s => new EvaluatedDesiredSkill(s,
                                evaluation.SkillsRanks[s.SkillId]))
                            .ToList(),
                        SalaryRank = evaluation.SalaryRank,
                    };
                }
                catch
                {
                    viewModel = new EvaluatedJobSeekerApplicant
                    {
                        Applicant = applicant,
                        IsEvaluated = false,
                        Educations = _context.EducationQualifications
                            .Include(e => e.FieldOfStudy)
                            .Where(e => e.JobVacancyId == applicant.JobVacancyId)
                            .Select(e => new EvaluatedEducationQualification(e))
                            .ToList(),
                        WorkExperiences = _context.WorkExperienceQualifications
                            .Include(w => w.JobTitle)
                            .Where(w => w.JobVacancyId == applicant.JobVacancyId)
                            .Select(w => new EvaluatedWorkExperienceQualification(w))
                            .ToList(),
                        DesiredSkills = _context.DesiredSkills
                            .Include(s => s.Skill)
                            .Where(s => s.JobVacancyId == applicant.JobVacancyId)
                            .Select(s => new EvaluatedDesiredSkill(s))
                            .ToList(),
                    };
                }

                return View("DetailsForJobSeeker", viewModel);
            }

            applicant = await _context.Applicants
                .Include(a => a.Recruiter)
                .SingleOrDefaultAsync(a => a.Id == id);

            if (applicant == null) return NotFound();
            if (applicant.Recruiter.UserName != User.Identity.Name) return BadRequest();

            applicant.Resume = await _context.Resumes
                .Include(r => r.Educations).ThenInclude(e => e.FieldOfStudy)
                .Include(r => r.Educations).ThenInclude(e => e.School)
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

            if (!((ApplicantStatus)applicant.Status).IsNextState((ApplicantStatus)status, userType))
                return BadRequest();

            await ProcessChangingStatus(applicant, status);

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [Route("Delete")]
        [Authorize(Roles = "JobSeeker")]
        public async Task<IActionResult> Delete(string id)
        {
            var applicant = await _context.Applicants
                .Include(a => a.JobSeeker)
                .SingleOrDefaultAsync(a => a.Id == id);

            if (applicant == null) return NotFound();
            if (applicant.JobSeeker.UserName != User.Identity.Name ||
                ((ApplicantStatus)applicant.Status).IsFinal()) return BadRequest();

            await AsyncHandler.DeleteApplicant(_context, _hubContext, applicant);
            return LocalRedirect("/");
        }

        private async Task ProcessChangingStatus(Applicant applicant, int newStatus)
        {
            JobVacancy jobVacancy;
            User recruiter;
            User jobSeeker;
            switch ((ApplicantStatus)newStatus)
            {
                case ApplicantStatus.DummyAccepted:
                case ApplicantStatus.DummyRejected:
                    if (applicant.Status != (int)ApplicantStatus.WaitingRecruiterDecision) break;
                    jobVacancy = await _context.JobVacancies
                        .SingleOrDefaultAsync(j => j.Id == applicant.JobVacancyId);
                    jobVacancy.AwaitingApplicants--;
                    break;
                case ApplicantStatus.AcceptMeeting:
                case ApplicantStatus.RejectMeeting:
                    jobVacancy = await _context.JobVacancies
                        .SingleOrDefaultAsync(j => j.Id == applicant.JobVacancyId);
                    jobVacancy.AwaitingApplicants--;
                    if (jobVacancy.AwaitingApplicants == 0)
                    {
                        jobVacancy.Status = (int) JobVacancyStatus.Finished;
                    }

                    recruiter = await _userManager.FindByIdAsync(applicant.RecruiterId);
                    jobVacancy = await _context.JobVacancies
                        .SingleOrDefaultAsync(j => j.Id == applicant.JobVacancyId);
                    jobSeeker = await _userManager.FindByIdAsync(applicant.JobSeekerId);
                    await _context.SendNotificationAsync(_hubContext, new Notification
                    {
                        Type = (int)((ApplicantStatus)newStatus).GetNotificationType(),
                        EntityId = applicant.Id,
                        Peer1 = $"{jobSeeker.FirstName} {jobSeeker.LastName}",
                        Peer2 = jobVacancy.Title,
                    }, recruiter);
                    break;
                case ApplicantStatus.WaitingRecruiterDecision:
                    recruiter = await _userManager.FindByIdAsync(applicant.RecruiterId);
                    jobVacancy = await _context.JobVacancies
                        .SingleOrDefaultAsync(j => j.Id == applicant.JobVacancyId);
                    jobSeeker = await _userManager.FindByIdAsync(applicant.JobSeekerId);
                    await _context.SendNotificationAsync(_hubContext, new Notification
                    {
                        Type = (int)((ApplicantStatus)newStatus).GetNotificationType(),
                        EntityId = applicant.Id,
                        Peer1 = $"{jobSeeker.FirstName} {jobSeeker.LastName}",
                        Peer2 = jobVacancy.Title,
                    }, recruiter);
                    break;
                case ApplicantStatus.RejectedRecommendation:
                case ApplicantStatus.AcceptedByRecruiter:
                case ApplicantStatus.RejectedByRecruiter:
                    break;
                case ApplicantStatus.PendingRecommendation:
                case ApplicantStatus.Other:
                    return;
                default:
                    return;
            }
            applicant.Status = newStatus;
            await _context.SaveChangesAsync();
        }
    }
}

