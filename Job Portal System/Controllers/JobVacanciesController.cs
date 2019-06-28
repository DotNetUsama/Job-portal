using System;
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
using Job_Portal_System.ViewModels.Companies;
using Job_Portal_System.ViewModels.JobVacancies;
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

        [HttpGet]
        [Route("Index")]
        public IActionResult Index(string tab, string q)
        {
            var isRecruiter = User.IsInRole("Recruiter");

            tab = tab ?? (isRecruiter ? "my-jobs" : "interesting");

            IQueryable<JobVacancy> jobVacanciesQueryable;
            switch (tab)
            {
                case "recent":
                    jobVacanciesQueryable = RecentJobVacancies();
                    break;
                case "interesting":
                    jobVacanciesQueryable = InterestingJobVacancies();
                    break;
                case "my-jobs":
                    jobVacanciesQueryable = OwnJobVacancies();
                    break;
                default:
                    return NotFound();
            }

            var jobVacancies = jobVacanciesQueryable
                .Where(j => string.IsNullOrEmpty(q) || j.Title.Contains(q) || j.JobTitle.Title.Contains(q));

            return View("JobVacanciesIndex", new JobVacanciesIndexViewModel
            {
                IsRecruiter = isRecruiter,
                ActiveTab = tab,
                Query = q,
                Count = jobVacancies.Count(),
                JobVacancies = jobVacancies
                    .Include(j => j.JobTitle)
                    .Include(j => j.CompanyDepartment).ThenInclude(d => d.City).ThenInclude(c => c.State)
                    .Include(j => j.CompanyDepartment).ThenInclude(d => d.Company)
                    .Take(20)
                    .Select(j => new JobVacancyGeneralViewModel
                    {
                        Id = j.Id,
                        Title = j.Title,
                        CreatedAt = j.PublishedAt,
                        IsRemote = j.DistanceLimit == 0,
                        MinSalary = j.MinSalary,
                        MaxSalary = j.MaxSalary,
                        JobTitle = j.JobTitle.Title,
                        Location = $"{j.CompanyDepartment.City.State.Name}, {j.CompanyDepartment.City.Name}",
                        Company = j.CompanyDepartment.Company.Name,
                        DesiredSkills = _context.DesiredSkills
                            .Where(s => s.JobVacancyId == j.Id)
                            .Select(s => s.Skill.Title),
                    }),
            });
        }

        [HttpGet]
        [Route("Details")]
        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var jobVacancyViewModel = _context.JobVacancies
                .Where(j => j.Id == id)
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
                            Type = (QualificationType) s.Type,
                        }),
                    WorkExperienceQualifications = _context.WorkExperienceQualifications
                        .Where(w => w.JobVacancyId == j.Id)
                        .Select(w => new QualificationViewModel
                        {
                            Title = w.JobTitle.Title,
                            Years = w.MinimumYears,
                            Type = (QualificationType) w.Type,
                        }),
                    EducationQualifications = _context.EducationQualifications
                        .Where(e => e.JobVacancyId == j.Id)
                        .Select(e => new QualificationViewModel
                        {
                            Title = e.FieldOfStudy.Title,
                            Years = e.MinimumYears,
                            Type = (QualificationType) e.Type,
                        }),
                    Company = _context.Companies
                        .Where(c => c.Id == j.CompanyDepartment.CompanyId)
                        .Select(c => new CompanyFullViewModel
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Logo = c.Logo,
                            Description = c.Description,
                            EmployeesNum = c.EmployeesNum,
                            FoundedYear = c.FoundedYear,
                            Type = c.Type,
                            JobVacancies = _context.JobVacancies
                                .Where(cj => cj.CompanyDepartment.CompanyId == c.Id)
                                .Select(cj => new JobVacancyGeneralViewModel
                                {
                                    Id = cj.Id,
                                    Title = cj.Title,
                                    CreatedAt = cj.PublishedAt,
                                    IsRemote = cj.DistanceLimit == 0,
                                    MinSalary = cj.MinSalary,
                                    MaxSalary = cj.MaxSalary,
                                    JobTitle = cj.JobTitle.Title,
                                    Location = $"{cj.CompanyDepartment.City.State.Name}, {cj.CompanyDepartment.City.Name}",
                                    Company = c.Name,
                                    DesiredSkills = _context.DesiredSkills
                                        .Where(s => s.JobVacancyId == cj.Id)
                                        .Select(s => s.Skill.Title),
                                }),
                        })
                        .First(),
                    RelatedJobVacancies = _context.JobVacancies
                        .Where(rj => rj.JobTitleId == j.JobTitleId)
                        .Select(rj => new JobVacancyGeneralViewModel
                        {
                            Id = rj.Id,
                            Title = rj.Title,
                            CreatedAt = rj.PublishedAt,
                            IsRemote = rj.DistanceLimit == 0,
                            MinSalary = rj.MinSalary,
                            MaxSalary = rj.MaxSalary,
                            JobTitle = rj.JobTitle.Title,
                            Location = $"{rj.CompanyDepartment.City.State.Name}, {rj.CompanyDepartment.City.Name}",
                            Company = rj.CompanyDepartment.Company.Name,
                            DesiredSkills = _context.DesiredSkills
                                .Where(s => s.JobVacancyId == rj.Id)
                                .Select(s => s.Skill.Title),
                        }),
                    JobTypes = _context.JobVacancyJobTypes
                        .Where(t => t.JobVacancyId == j.Id)
                        .Select(t => t.JobType),
                })
                .First();

            if (jobVacancyViewModel == null) return NotFound();
            return View("JobVacancyDetails", jobVacancyViewModel);
        }

        [HttpGet]
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

            if (jobVacancy.Method == (int)JobVacancyMethod.Recommendation)
            {
                var pendingApplicants = _context.Applicants
                    .Where(a => a.JobVacancyId == jobVacancy.Id &&
                                a.Status == (int)ApplicantStatus.PendingRecommendation);
                foreach (var pendingApplicant in pendingApplicants)
                {
                    pendingApplicant.Status = (int)ApplicantStatus.RejectedByRecruiter;
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "JobVacancies", new { id });
        }

        [HttpGet]
        [Route("Delete")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var jobVacancy = await _context.JobVacancies
                .Include(j => j.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (jobVacancy == null) return NotFound();

            if (jobVacancy.User.UserName != User.Identity.Name) return BadRequest();

            if (jobVacancy.Method != (int)JobVacancyMethod.Submission &&
                (jobVacancy.Method != (int)JobVacancyMethod.Recommendation ||
                 jobVacancy.Status == (int)JobVacancyStatus.Open)) return BadRequest();

            await AsyncHandler.DeleteJobVacancy(_context, _hubContext, jobVacancy);

            return RedirectToAction("Index");
        }

        [HttpGet]
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
                .FirstOrDefaultAsync(j => j.Id == id);

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
                .FirstOrDefaultAsync(j => j.User.UserName == User.Identity.Name);

            if (jobSeeker == null) return BadRequest();

            var resume = _context.Resumes
                .FirstOrDefault(r => r.JobSeekerId == jobSeeker.Id);

            if (resume == null || !resume.IsSeeking ||
                _context.Applicants.Any(a => a.JobVacancyId == jobVacancy.Id
                                             && a.JobSeekerId == jobSeeker.User.Id))
            {
                return BadRequest();
            }

            await AsyncHandler.SubmitToJobVacancy(_context, _hubContext, jobVacancy, resume);

            return RedirectToAction("Details", "JobVacancies", new { id });
        }

        [HttpGet]
        [Route("FinalDecision")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> FinalDecision(string id)
        {
            var jobVacancy = await _context.JobVacancies
                .FirstOrDefaultAsync(j => j.Id == id);

            if (jobVacancy == null) return NotFound();
            if (jobVacancy.Status != (int)JobVacancyStatus.Closed ||
                jobVacancy.AwaitingApplicants != 0) return BadRequest();

            var jobVacancyOwner = await _userManager.FindByIdAsync(jobVacancy.UserId);
            if (jobVacancyOwner.UserName != User.Identity.Name) return BadRequest();

            var applicantsCount = _context.Applicants
                .Count(a => a.JobVacancyId == jobVacancy.Id);
            if (applicantsCount == 0) return BadRequest();

            jobVacancy.EducationQualifications = _context.EducationQualifications
                .Include(e => e.FieldOfStudy)
                .Where(e => e.JobVacancyId == jobVacancy.Id)
                .ToList();
            jobVacancy.WorkExperienceQualifications = _context.WorkExperienceQualifications
                .Include(w => w.JobTitle)
                .Where(w => w.JobVacancyId == jobVacancy.Id)
                .ToList();
            jobVacancy.DesiredSkills = _context.DesiredSkills
                .Include(s => s.Skill)
                .Where(s => s.JobVacancyId == jobVacancy.Id)
                .ToList();

            await AsyncHandler.FinalDecideOnApplicants(_env, _context, _hubContext, jobVacancy);

            return RedirectToAction("Details", "JobVacancies", new { id });
        }

        [HttpGet]
        [Route("Search")]
        public IActionResult Search(string query, int p = 1)
        {
            var normalizedTitle = query.ToLower().Split(" - ").Last();
            var jobTitleSynsetId = _context.JobTitles
                .FirstOrDefault(j => j.NormalizedTitle == normalizedTitle)?.JobTitleSynsetId;

            if (jobTitleSynsetId == null)
            {
                var similarities = SimilaritiesOperator.GetSimilarities(normalizedTitle, _env);
                foreach (var similarity in similarities)
                {
                    jobTitleSynsetId = _context.JobTitles
                        .FirstOrDefault(j => j.NormalizedTitle == similarity)?.JobTitleSynsetId;
                    if (jobTitleSynsetId != null) break;
                }
            }

            var jobVacancies = new List<JobVacancy>();

            if (jobTitleSynsetId != null)
            {
                jobVacancies = _context.JobVacancies
                    .Where(j => j.JobTitle.JobTitleSynsetId == jobTitleSynsetId)
                    .ToList();
            }

            var pager = new Pager(jobVacancies.Count, p, 15);

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

        private IQueryable<JobVacancy> RecentJobVacancies()
        {
            return _context.JobVacancies
                .Where(j => j.Status == (int) JobVacancyStatus.Open)
                .OrderByDescending(r => r.PublishedAt);
        }

        private IQueryable<JobVacancy> InterestingJobVacancies()
        {
            return _context.JobVacancies
                .Where(j => j.Status == (int)JobVacancyStatus.Open)
                .OrderByDescending(r => r.PublishedAt)
                .Take(200)
                .OrderBy(r => Guid.NewGuid());
        }

        private IQueryable<JobVacancy> OwnJobVacancies()
        {
            return _context.JobVacancies
                .Where(j => j.UserId == _userManager.GetUserId(User));
        }
    }
}
