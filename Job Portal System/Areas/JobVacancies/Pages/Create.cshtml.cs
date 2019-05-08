using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;
using Job_Portal_System.Handlers;
using Job_Portal_System.Models;
using Job_Portal_System.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace Job_Portal_System.Areas.JobVacancies.Pages
{
    //[Authorize(Roles = "Recruiter")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SignalRHub> _hubContext;
        private readonly UserManager<User> _userManager;

        public CreateModel(ApplicationDbContext context,
            IHubContext<SignalRHub> hubContext,
            UserManager<User> userManager)
        {
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        public class EducationInputModel
        {
            [Required]
            [EnumDataType(typeof(EducationDegree))]
            [Display(Name = "Degree")]
            public int Degree { get; set; }

            [Required]
            [EnumDataType(typeof(QualificationType))]
            [Display(Name = "Type")]
            public int Type { get; set; }

            [Required]
            [Display(Name = "Minimum years")]
            [Range(1, 10)]
            public int MinimumYears { get; set; }

            [Required]
            [HiddenInput]
            [Display(Name = "Field of study")]
            public string FieldOfStudyName { get; set; }

            [Required]
            [HiddenInput]
            public long FieldOfStudyId { get; set; }
        }

        public class WorkExperienceInputModel
        {
            [Required]
            [EnumDataType(typeof(QualificationType))]
            [Display(Name = "Type")]
            public int Type { get; set; }

            [Required]
            [Display(Name = "Minimum years")]
            [Range(1, 10)]
            public int MinimumYears { get; set; }

            [Required]
            [HiddenInput]
            [Display(Name = "Job title")]
            public string JobTitle { get; set; }

            [Required]
            [HiddenInput]
            public long JobTitleId { get; set; }
        }

        public class SkillInputModel
        {
            [Required]
            [EnumDataType(typeof(QualificationType))]
            [Display(Name = "Type")]
            public int Type { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Skill")]
            public string Skill { get; set; }

            [HiddenInput]
            public long? SkillId { get; set; }

            [Required]
            [Display(Name = "Minimum years")]
            [Range(1, 25)]
            public int MinimumYears { get; set; }
        }

        public class JobVacancyInputModel
        {
            [Required]
            [HiddenInput]
            [Display(Name = "Job title")]
            public string JobTitle { get; set; }

            [HiddenInput]
            public long JobTitleId { get; set; }

            [Required]
            [Display(Name = "Title")]
            public string Title { get; set; }

            [Required]
            [Display(Name = "Description")]
            [DataType(DataType.MultilineText)]
            public string Description { get; set; }

            [Required]
            [Display(Name = "Minimum salary")]
            [Range(100, 999999.99)]
            public double MinSalary { get; set; }

            [Required]
            [Display(Name = "Maximum salary")]
            [Range(100, 999999.99)]
            public double MaxSalary { get; set; }

            [Required]
            [Display(Name = "Required hires")]
            [Range(1, 50)]
            public int RequiredHires { get; set; }

            [Required]
            [EnumDataType(typeof(JobVacancyMethod))]
            [Display(Name = "Method")]
            public int Method { get; set; }

            public Dictionary<JobType, bool> JobTypes { get; set; }

            [HiddenInput]
            public string CompanyDepartmentId { get; set; }
        }

        public string CompanyId { get; set; }

        [BindProperty]
        public List<EducationInputModel> Educations { get; set; } =
            new List<EducationInputModel>
        {
            new EducationInputModel()
        };

        [BindProperty]
        public List<WorkExperienceInputModel> WorkExperiences { get; set; } =
            new List<WorkExperienceInputModel>
        {
            new WorkExperienceInputModel()
        };

        [BindProperty]
        public List<SkillInputModel> DesiredSkills { get; set; } = new List<SkillInputModel>
        {
            new SkillInputModel()
        };

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
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var recruiter = _context.Recruiters.SingleOrDefault(recruiterInDb => recruiterInDb.UserId == user.Id);
            var jobVacancy = new JobVacancy
            {
                Title = JobVacancyInfo.Title,
                Description = JobVacancyInfo.Description,
                MinSalary = JobVacancyInfo.MinSalary,
                MaxSalary = JobVacancyInfo.MaxSalary,
                RequiredHires = JobVacancyInfo.RequiredHires,
                Method = JobVacancyInfo.Method,
                JobTypes = new List<JobVacancyJobType>(),
                EducationQualifications = new List<EducationQualification>(),
                WorkExperienceQualifications = new List<WorkExperienceQualification>(),
                DesiredSkills = new List<DesiredSkill>(),
                User = user,
                Recruiter = recruiter,
            };
            PrepareLists();
            Educations.ForEach(education => AddEducation(jobVacancy, education));
            WorkExperiences.ForEach(workExperience => AddWorkExperience(jobVacancy, workExperience));
            DesiredSkills.ForEach(skill => AddSkill(jobVacancy, skill));
            AddJobTitle(jobVacancy);
            AddCompanyDepartment(jobVacancy);
            AddJobTypes(jobVacancy);
            _context.JobVacancies.Add(jobVacancy);
            if (JobVacancyInfo.Method == (int) JobVacancyMethod.Recommendation)
            {
                await AsyncHandler.Recommend(_context, _hubContext, jobVacancy);
                return Redirect("./Index");
            }

            await _context.SaveChangesAsync();
            return Redirect("./Index");
        }

        public void PrepareLists()
        {
            Educations.RemoveAt(Educations.Count - 1);
            WorkExperiences.RemoveAt(WorkExperiences.Count - 1);
            DesiredSkills.RemoveAt(DesiredSkills.Count - 1);
        }

        private void AddJobTypes(JobVacancy jobVacancy)
        {
            foreach (var jobVacancyInfoJobType in JobVacancyInfo.JobTypes)
            {
                if (jobVacancyInfoJobType.Value)
                    jobVacancy.JobTypes.Add(new JobVacancyJobType
                    {
                        JobType = (int)jobVacancyInfoJobType.Key,
                    });
            }
        }

        private void AddJobTitle(JobVacancy jobVacancy)
        {
            var jobTitle =
                _context.JobTitles.SingleOrDefault(jobTitleInDb => jobTitleInDb.Id == JobVacancyInfo.JobTitleId);
            if (jobTitle != null && jobTitle.Title != JobVacancyInfo.JobTitle)
            {
                _context.JobTitleSimilarities.FindOrAdd(new JobTitleSimilarity
                {
                    JobTitle = jobTitle,
                    SimilarTitle = new SimilarJobTitle
                    {
                        Title = JobVacancyInfo.JobTitle,
                    },
                }, similarityInDb =>
                    similarityInDb.JobTitle.Id == JobVacancyInfo.JobTitleId &&
                    similarityInDb.SimilarTitle.Title == JobVacancyInfo.JobTitle);
            }

            jobVacancy.JobTitle = jobTitle;
        }

        private void AddCompanyDepartment(JobVacancy jobVacancy)
        {
            jobVacancy.CompanyDepartment = _context.CompanyDepartments
                .SingleOrDefault(d => d.Id == JobVacancyInfo.CompanyDepartmentId);
        }

        private void AddEducation(JobVacancy jobVacancy, EducationInputModel education)
        {
            var fieldOfStudy =
                _context.FieldOfStudies.SingleOrDefault(fieldInDb => fieldInDb.Id == education.FieldOfStudyId);
            if (fieldOfStudy != null && fieldOfStudy.Title != education.FieldOfStudyName)
            {
                _context.FieldOfStudySimilarities.FindOrAdd(new FieldOfStudySimilarity
                {
                    FieldOfStudy = fieldOfStudy,
                    SimilarTitle = new SimilarFieldOfStudyTitle
                    {
                        Title = education.FieldOfStudyName,
                    },
                }, similarityInDb =>
                    similarityInDb.FieldOfStudy.Id == education.FieldOfStudyId &&
                    similarityInDb.SimilarTitle.Title == education.FieldOfStudyName);
            }

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
                _context.JobTitles.SingleOrDefault(jobTitleInDb => jobTitleInDb.Id == workExperience.JobTitleId);
            if (jobTitle != null && jobTitle.Title != workExperience.JobTitle)
            {
                _context.JobTitleSimilarities.FindOrAdd(new JobTitleSimilarity
                {
                    JobTitle = jobTitle,
                    SimilarTitle = new SimilarJobTitle
                    {
                        Title = workExperience.JobTitle,
                    },
                }, similarityInDb =>
                    similarityInDb.JobTitle.Id == workExperience.JobTitleId &&
                    similarityInDb.SimilarTitle.Title == workExperience.JobTitle);
            }

            jobVacancy.WorkExperienceQualifications.Add(new WorkExperienceQualification()
            {
                Type = workExperience.Type,
                MinimumYears = workExperience.MinimumYears,
                JobTitle = jobTitle,
            });
        }

        private void AddSkill(JobVacancy jobVacancy, SkillInputModel skillModel)
        {
            var skill = skillModel.SkillId == null
                ? new Skill { Title = skillModel.Skill }
                : _context.Skills.SingleOrDefault(skillInDb => skillInDb.Id == skillModel.SkillId);

            jobVacancy.DesiredSkills.Add(new DesiredSkill
            {
                Type = skillModel.Type,
                Skill = skill,
                MinimumYears = skillModel.MinimumYears,
            });
        }
    }
}