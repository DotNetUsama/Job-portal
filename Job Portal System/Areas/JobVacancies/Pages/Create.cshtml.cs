using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Areas.JobVacancies.Pages.InputModels;
using Job_Portal_System.BackgroundTasking;
using Job_Portal_System.Data;
using Job_Portal_System.Dependencies;
using Job_Portal_System.Enums;
using Job_Portal_System.Handlers;
using Job_Portal_System.Models;
using Job_Portal_System.SignalR;
using Job_Portal_System.Utilities.RankingSystem;
using Job_Portal_System.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Job_Portal_System.Areas.JobVacancies.Pages
{
    [Authorize(Roles = "Recruiter")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IBackgroundOperator _backgroundOperator;
        //private readonly IServiceScopeFactory _serviceScopeFactory;
        //private readonly BackgroundTaskQueue _queue;

        public CreateModel(ApplicationDbContext context,
            UserManager<User> userManager,
            IBackgroundOperator backgroundOperator
            //,
            //IServiceScopeFactory serviceScopeFactory,
            //BackgroundTaskQueue queue
            )
        {
            _context = context;
            _userManager = userManager;
            _backgroundOperator = backgroundOperator;
            //_serviceScopeFactory = serviceScopeFactory;
            //_queue = queue;
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
            AddJobTitle(jobVacancy);
            AddJobTypes(jobVacancy);
            _context.JobVacancies.Add(jobVacancy);
            if (JobVacancyInfo.Method == (int) JobVacancyMethod.Recommendation)
            {
                _backgroundOperator.Recommend(jobVacancy);

                //_queue.QueueBackgroundWorkItem(async token =>
                //{
                //    using (var scope = _serviceScopeFactory.CreateScope())
                //    {
                //        var scopedServices = scope.ServiceProvider;
                //        var context = scopedServices.GetRequiredService<ApplicationDbContext>();
                //        var hubContext = scopedServices.GetRequiredService<IHubContext<SignalRHub>>();
                //        var env = scopedServices.GetRequiredService<IHostingEnvironment>();
                //        await AsyncHandler.Recommend(context, env, hubContext, jobVacancy);
                //    }
                //});
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

        private void AddJobTitle(JobVacancy jobVacancy)
        {
            var jobTitle = 
                _context.JobTitles.SingleOrDefault(jobTitleInDb => 
                    jobTitleInDb.NormalizedTitle == JobVacancyInfo.JobTitle.ToLower()) ??
                new JobTitle
                {
                    Title = JobVacancyInfo.JobTitle,
                    NormalizedTitle = JobVacancyInfo.JobTitle.ToLower(),
                };

            jobVacancy.JobTitle = jobTitle;
        }

        private void AddEducation(JobVacancy jobVacancy, EducationInputModel education)
        {
            var fieldOfStudy =
                _context.FieldOfStudies.SingleOrDefault(fieldInDb => 
                    fieldInDb.NormalizedTitle == education.FieldOfStudy) ??
                new FieldOfStudy
                {
                    Title = education.FieldOfStudy,
                    NormalizedTitle = education.FieldOfStudy.ToLower(),
                };

            jobVacancy.EducationQualifications.Add(new EducationQualification
            {
                Type = education.Type,
                Degree = education.Degree,
                MinimumYears = education.MinimumYears,
                FieldOfStudy = fieldOfStudy,
            });
        }

        private void AddWorkExperience(JobVacancy jobVacancy, WorkExperienceInputModel workExperience)
        {
            var jobTitle =
                _context.JobTitles.SingleOrDefault(jobTitleInDb => 
                    jobTitleInDb.NormalizedTitle == workExperience.JobTitle.ToLower()) ??
                new JobTitle
                {
                    Title = workExperience.JobTitle,
                    NormalizedTitle = workExperience.JobTitle.ToLower(),
                };

            jobVacancy.WorkExperienceQualifications.Add(new WorkExperienceQualification()
            {
                Type = workExperience.Type,
                MinimumYears = workExperience.MinimumYears,
                JobTitle = jobTitle,
            });
        }

        private void AddSkill(JobVacancy jobVacancy, SkillInputModel skillModel)
        {
            var skill =
                _context.Skills.SingleOrDefault(skillInDb => 
                    skillInDb.NormalizedTitle == skillModel.Skill) ??
                new Skill
                {
                    Title = skillModel.Skill,
                    NormalizedTitle = skillModel.Skill.ToLower(),
                };

            jobVacancy.DesiredSkills.Add(new DesiredSkill
            {
                Type = skillModel.Type,
                Skill = skill,
                MinimumYears = skillModel.MinimumYears,
            });
        }
    }
}