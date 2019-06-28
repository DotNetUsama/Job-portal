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
using Job_Portal_System.ViewModels.Applicants;
using Job_Portal_System.ViewModels.Companies;
using Job_Portal_System.ViewModels.JobVacancies;
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
        [Route("Index")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> Index(string jobVacancyId)
        {
            if (string.IsNullOrEmpty(jobVacancyId)) return NotFound();
            var jobVacancy = await _context.JobVacancies
                .Include(j => j.CompanyDepartment).ThenInclude(d => d.City).ThenInclude(c => c.State)
                .Include(j => j.JobTitle)
                .FirstOrDefaultAsync(j => j.Id == jobVacancyId);

            if (jobVacancy == null) return NotFound();
            if (jobVacancy.UserId != _userManager.GetUserId(User)) return BadRequest();

            return View("ApplicantsIndexForRecruiter", new ApplicantsForRecruiterIndexViewModel
            {
                JobVacancyId = jobVacancy.Id,
                JobVacancyTitle = jobVacancy.Title,
                ApplicantsCount = _context.Applicants.Count(a => a.JobVacancyId == jobVacancy.Id),
                Status = (JobVacancyStatus) jobVacancy.Status,
                JobTitle = jobVacancy.JobTitle.Title,
                IsRemote = jobVacancy.DistanceLimit == 0,
                Location = $"{jobVacancy.CompanyDepartment.City.State.Name}, {jobVacancy.CompanyDepartment.City.Name}, {jobVacancy.CompanyDepartment.DetailedAddress}",
                Applicants = _context.Applicants
                    .Where(a => a.JobVacancyId == jobVacancy.Id)
                    .Select(a => new ApplicantForRecruiterGeneralViewModel
                    {
                        Id = a.Id,
                        OwnerName = $"{a.JobSeeker.FirstName} {a.JobSeeker.LastName}",
                        WorksCount = a.Resume.WorkExperiences.Count,
                        EducationsCount = a.Resume.Educations.Count,
                        SkillsCount = a.Resume.OwnedSkills.Count,
                        Location = $"{a.JobSeeker.City.State.Name}, {a.JobSeeker.City.Name}",
                        Status = (ApplicantStatus)a.Status,
                        Skills = a.Resume.OwnedSkills.Select(s => s.Skill.Title),
                        SubmittedAt = a.SubmittedAt,
                    }),
            });
        }

        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Details(string id)
        {
            var applicant = await _context.Applicants
                .FirstOrDefaultAsync(a => a.Id == id);

            if (applicant == null) return NotFound();

            if (!User.IsInRole("Recruiter") && !User.IsInRole("JobSeeker")) return BadRequest();

            if (User.IsInRole("JobSeeker"))
            {
                if (applicant.JobSeekerId != _userManager.GetUserId(User)) return BadRequest();

                Evaluation evaluation = null;
                try
                {
                    evaluation = FilesManager.Read<Evaluation>(_env, "Evaluations", applicant.Id);
                }
                catch
                {
                    // ignored
                }
                return View("DetailsForJobSeeker", new ApplicantForJobSeekerFullViewModel
                {
                    Id = applicant.Id,
                    Status = (ApplicantStatus)applicant.Status,
                    SubmittedAt = applicant.SubmittedAt,
                    IsSalaryWeak = evaluation != null && evaluation.SalaryRank.IsWeakness,
                    JobVacancy = _context.JobVacancies
                    .Where(j => j.Id == applicant.JobVacancyId)
                    .Select(j => new JobVacancyFullViewModel
                    {
                        Id = j.Id,
                        Title = j.Title,
                        IsRemote = j.DistanceLimit == 0,
                        MinSalary = j.MinSalary,
                        MaxSalary = j.MaxSalary,
                        JobTitle = j.JobTitle.Title,
                        Location = $"{j.CompanyDepartment.City.State.Name}, {j.CompanyDepartment.City.Name}",
                        Description = j.Description,
                        DesiredSkills = _context.DesiredSkills
                            .Where(s => s.JobVacancyId == j.Id)
                            .Select(s => new QualificationViewModel
                            {
                                Title = s.Skill.Title,
                                Years = s.MinimumYears,
                                Type = (QualificationType)s.Type,
                                IsWeakness = evaluation != null && evaluation.SkillsRanks[s.Skill.SkillSynsetId].IsWeakness,
                            }),
                        WorkExperienceQualifications = _context.WorkExperienceQualifications
                            .Where(w => w.JobVacancyId == j.Id)
                            .Select(w => new QualificationViewModel
                            {
                                Title = w.JobTitle.Title,
                                Years = w.MinimumYears,
                                Type = (QualificationType)w.Type,
                                IsWeakness = evaluation != null && evaluation.WorkExperiencesRanks[w.JobTitle.JobTitleSynsetId].IsWeakness,
                            }),
                        EducationQualifications = _context.EducationQualifications
                            .Where(e => e.JobVacancyId == j.Id)
                            .Select(e => new QualificationViewModel
                            {
                                Title = e.FieldOfStudy.Title,
                                Years = e.MinimumYears,
                                Type = (QualificationType)e.Type,
                                IsWeakness = evaluation != null && evaluation.EducationsRanks[e.FieldOfStudy.FieldOfStudySynsetId].IsWeakness,
                            }),
                        Company = _context.Companies
                            .Where(c => c.Id == j.CompanyDepartment.CompanyId)
                            .Select(c => new CompanyFullViewModel
                            {
                                Id = c.Id,
                                Name = c.Name,
                                Logo = c.Logo,
                                EmployeesNum = c.EmployeesNum,
                                FoundedYear = c.FoundedYear,
                                Type = c.Type,
                            })
                            .First(),
                        JobTypes = _context.JobVacancyJobTypes
                            .Where(t => t.JobVacancyId == j.Id)
                            .Select(t => t.JobType),
                    })
                    .First(),

                });
            }

            if (applicant.RecruiterId != _userManager.GetUserId(User)) return BadRequest();

            return View("DetailsForRecruiter", new ApplicantForRecruiterFullViewModel
            {
                Id = applicant.Id,
                Status = (ApplicantStatus) applicant.Status,
                SubmittedAt = applicant.SubmittedAt,
                Resume = await _context.Resumes
                    .Include(r => r.Educations).ThenInclude(e => e.FieldOfStudy)
                    .Include(r => r.Educations).ThenInclude(e => e.School)
                    .Include(r => r.WorkExperiences).ThenInclude(w => w.JobTitle)
                    .Include(r => r.WorkExperiences).ThenInclude(w => w.Company)
                    .Include(r => r.OwnedSkills).ThenInclude(s => s.Skill)
                    .Include(r => r.User).ThenInclude(u => u.City).ThenInclude(c => c.State)
                    .FirstOrDefaultAsync(r => r.Id == applicant.ResumeId),
        });
        }

        [HttpGet]
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

        [HttpGet]
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
                        jobVacancy.Status = (int)JobVacancyStatus.Finished;
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
                    jobVacancy.AwaitingApplicants++;
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

