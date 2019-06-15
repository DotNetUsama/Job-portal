using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Areas.JobVacancies.Pages.InputModels;
using Job_Portal_System.Data;
using Job_Portal_System.Dependencies;
using Job_Portal_System.Enums;
using Job_Portal_System.Handlers;
using Job_Portal_System.Models;
using Job_Portal_System.SignalR;
using Job_Portal_System.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace Job_Portal_System.Areas.JobVacancies.Pages
{
    [Authorize(Roles = "Recruiter")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<SignalRHub> _hubContext;
        private readonly ITermsManager _termsManager;

        public CreateModel(ApplicationDbContext context,
            UserManager<User> userManager,
            IHubContext<SignalRHub> hubContext,
            ITermsManager termsManager)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
            _termsManager = termsManager;
        }

        [BindProperty]
        public string CompanyId { get; set; }

        [BindProperty]
        [MinimumCount(1, ErrorMessage = "You should specify at least one education qualification")]
        public List<EducationInputModel> Educations { get; set; }
        public EducationInputModel Education { get; set; }

        [BindProperty]
        [MinimumCount(1, ErrorMessage = "You should specify at least one work experience qualification")]
        public List<WorkExperienceInputModel> WorkExperiences { get; set; }
        public WorkExperienceInputModel WorkExperience { get; set; }

        [BindProperty]
        [MinimumCount(1, ErrorMessage = "You should specify at least one desired skill")]
        public List<SkillInputModel> DesiredSkills { get; set; }
        public SkillInputModel DesiredSkill { get; set; }

        [BindProperty]
        public JobVacancyInputModel JobVacancyInfo { get; set; } = new JobVacancyInputModel
        {
            JobTypes = JobTypeMethods.GetDictionary(),
        };

        public IActionResult OnGet()
        {
            var recruiter = _context.Recruiters
                .SingleOrDefault(r => r.User.UserName == User.Identity.Name);

            if (recruiter == null)
            {
                return BadRequest();
            }
            CompanyId = recruiter.CompanyId;
            return Page();
        }

        public async Task<IActionResult> OnPostCreateJobVacancyAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var recruiter = _context.Recruiters
                .SingleOrDefault(recruiterInDb => recruiterInDb.UserId == user.Id);
            var jobVacancy = new JobVacancy
            {
                JobTitle = _termsManager.GetJobTitle(JobVacancyInfo.JobTitle),
                Title = JobVacancyInfo.Title,
                Description = JobVacancyInfo.Description,
                DistanceLimit = JobVacancyInfo.DistanceLimit,
                MinSalary = JobVacancyInfo.MinSalary,
                MaxSalary = JobVacancyInfo.MaxSalary,
                RequiredHires = JobVacancyInfo.RequiredHires,
                Method = JobVacancyInfo.Method,
                JobTypes = new List<JobVacancyJobType>(),
                EducationQualifications = new List<EducationQualification>(),
                WorkExperienceQualifications = new List<WorkExperienceQualification>(),
                DesiredSkills = new List<DesiredSkill>(),
                CompanyDepartmentId = JobVacancyInfo.CompanyDepartmentId,
                User = user,
                Recruiter = recruiter,
            };
            Educations?.ForEach(education => AddEducation(jobVacancy, education));
            WorkExperiences?.ForEach(workExperience => AddWorkExperience(jobVacancy, workExperience));
            DesiredSkills?.ForEach(skill => AddSkill(jobVacancy, skill));
            AddJobTypes(jobVacancy);
            _context.JobVacancies.Add(jobVacancy);
            if (JobVacancyInfo.Method == (int)JobVacancyMethod.Recommendation)
            {
                await AsyncHandler.Recommend(_context, _hubContext, jobVacancy);
                return Redirect("./Index");
            }

            await _context.SaveChangesAsync();
            return Redirect("./Index");
        }

        private void AddJobTypes(JobVacancy jobVacancy)
        {
            foreach (var (key, value) in JobVacancyInfo.JobTypes)
            {
                if (value)
                    jobVacancy.JobTypes.Add(new JobVacancyJobType
                    {
                        JobType = (int)key,
                    });
            }
        }

        private void AddEducation(JobVacancy jobVacancy, EducationInputModel education)
        {
            jobVacancy.EducationQualifications.Add(new EducationQualification
            {
                Type = education.Type,
                Degree = education.Degree,
                MinimumYears = education.MinimumYears,
                FieldOfStudy = _termsManager.GetFieldOfStudy(education.FieldOfStudy),
            });
        }

        private void AddWorkExperience(JobVacancy jobVacancy, WorkExperienceInputModel workExperience)
        {
            jobVacancy.WorkExperienceQualifications.Add(new WorkExperienceQualification()
            {
                Type = workExperience.Type,
                MinimumYears = workExperience.MinimumYears,
                JobTitle = _termsManager.GetJobTitle(workExperience.JobTitle),
            });
        }

        private void AddSkill(JobVacancy jobVacancy, SkillInputModel skill)
        {
            jobVacancy.DesiredSkills.Add(new DesiredSkill
            {
                Type = skill.Type,
                Skill = _termsManager.GetSkill(skill.Skill),
                MinimumYears = skill.MinimumYears,
            });
        }
    }
}