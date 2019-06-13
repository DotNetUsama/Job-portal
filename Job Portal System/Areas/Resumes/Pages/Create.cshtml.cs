using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Areas.Resumes.Pages.InputModels;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Job_Portal_System.Areas.Resumes.Pages
{
    [Authorize(Roles = "JobSeeker")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CreateModel(ApplicationDbContext context,
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public List<EducationInputModel> Educations { get; set; }
        public EducationInputModel Education { get; set; }

        [BindProperty]
        public List<WorkExperienceInputModel> WorkExperiences { get; set; }
        public WorkExperienceInputModel WorkExperience { get; set; }

        [BindProperty]
        public List<SkillInputModel> OwnedSkills { get; set; }
        public SkillInputModel OwnedSkill { get; set; }

        [BindProperty]
        public List<JobTitleInputModel> SeekedJobTitles { get; set; }
        public JobTitleInputModel SeekedJobTitle { get; set; }

        [BindProperty]
        public ResumeInputModel ResumeInfo { get; set; } = new ResumeInputModel
        {
            JobTypes = JobTypeMethods.GetDictionary(),
        };

        public IActionResult OnGet()
        {
            var resumeInDb = _context.Resumes
                .SingleOrDefault(resume => resume.User.UserName == User.Identity.Name);
            if (resumeInDb != null)
            {
                return Redirect("./Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCreateResumeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var jobSeeker = _context.JobSeekers.SingleOrDefault(jobSeekerInDb => jobSeekerInDb.UserId == user.Id);
            var resume = new Resume
            {
                MinSalary = ResumeInfo.MinSalary,
                MovingDistanceLimit = ResumeInfo.MovingDistanceLimit,
                IsPublic = ResumeInfo.IsPublic,
                IsSeeking = ResumeInfo.IsSeeking,
                Biography = ResumeInfo.Biography,
                JobTypes = new List<ResumeJobType>(),
                Educations = new List<Education>(),
                WorkExperiences = new List<WorkExperience>(),
                OwnedSkills = new List<OwnedSkill>(),
                SeekedJobTitles = new List<SeekedJobTitle>(),
                User = user,
                JobSeeker = jobSeeker,
            };
            Educations?.ForEach(education => AddEducation(resume, education));
            WorkExperiences?.ForEach(workExperience => AddWorkExperience(resume, workExperience));
            OwnedSkills?.ForEach(skill => AddSkill(resume, skill));
            SeekedJobTitles?.ForEach(jobTitle => AddSeekedJobTitle(resume, jobTitle));
            AddJobTypes(resume);
            _context.Resumes.Add(resume);
            await _context.SaveChangesAsync();

            return Redirect("./Index");
        }

        private void AddJobTypes(Resume resume)
        {
            foreach (var (key, value) in ResumeInfo.JobTypes)
            {
                if (value)
                    resume.JobTypes.Add(new ResumeJobType
                    {
                        JobType = (int)key,
                    });
            }
        }

        private void AddEducation(Resume resume, EducationInputModel education)
        {
            var school =
                _context.Schools
                    .SingleOrDefault(schoolInDb => string.Equals(schoolInDb.Name,
                        education.School, StringComparison.OrdinalIgnoreCase)) ??
                new School
                {
                    Name = education.School,
                };

            var fieldOfStudy =
                _context.FieldOfStudies.SingleOrDefault(fieldInDb => 
                        fieldInDb.NormalizedTitle == education.FieldOfStudy.ToLower()) ??
                new FieldOfStudy
                {
                    Title = education.FieldOfStudy,
                    NormalizedTitle = education.FieldOfStudy.ToLower(),
                };

            resume.Educations.Add(new Education
            {
                StartDate = education.StartDate,
                EndDate = education.EndDate,
                Degree = education.Degree,
                FieldOfStudy = fieldOfStudy,
                School = school,
            });
        }

        private void AddWorkExperience(Resume resume, WorkExperienceInputModel workExperience)
        {
            var company =
                _context.Companies
                    .SingleOrDefault(companyInDb => string.Equals(companyInDb.Name,
                        workExperience.Company, StringComparison.OrdinalIgnoreCase)) ??
                new Company
                {
                    Name = workExperience.Company,
                    Approved = false,
                };

            var jobTitle =
                _context.JobTitles.SingleOrDefault(jobTitleInDb => 
                    jobTitleInDb.NormalizedTitle == workExperience.JobTitle.ToLower()) ??
                new JobTitle
                {
                    Title = workExperience.JobTitle,
                    NormalizedTitle = workExperience.JobTitle.ToLower(),
                };

            resume.WorkExperiences.Add(new WorkExperience()
            {
                StartDate = workExperience.StartDate,
                EndDate = workExperience.EndDate,
                Company = company,
                JobTitle = jobTitle,
                Description = workExperience.Description,
            });
        }

        private void AddSkill(Resume resume, SkillInputModel skillModel)
        {
            var skill =
                _context.Skills.SingleOrDefault(skillInDb => 
                        skillInDb.NormalizedTitle == skillModel.Skill.ToLower()) ??
                new Skill
                {
                    Title = skillModel.Skill,
                    NormalizedTitle = skillModel.Skill.ToLower(),
                };

            resume.OwnedSkills.Add(new OwnedSkill
            {
                Skill = skill,
                Years = skillModel.Years,
            });
        }

        private void AddSeekedJobTitle(Resume resume, JobTitleInputModel jobTitleModel)
        {
            var jobTitle =
                _context.JobTitles.SingleOrDefault(jobTitleInDb => 
                    jobTitleInDb.NormalizedTitle == jobTitleModel.JobTitle.ToLower()) ??
                new JobTitle
                {
                    Title = jobTitleModel.JobTitle,
                    NormalizedTitle = jobTitleModel.JobTitle.ToLower(),
                };

            resume.SeekedJobTitles.Add(new SeekedJobTitle
            {
                JobTitle = jobTitle,
            });
        }
    }
}