using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Areas.Resumes.Pages.InputModels;
using Job_Portal_System.Data;
using Job_Portal_System.Dependencies;
using Job_Portal_System.Enums;
using Job_Portal_System.Models;
using Job_Portal_System.Validation;
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
        private readonly ITermsManager _termsManager;

        public CreateModel(ApplicationDbContext context,
            UserManager<User> userManager,
            ITermsManager termsManager)
        {
            _context = context;
            _userManager = userManager;
            _termsManager = termsManager;
        }

        [BindProperty]
        [MinimumCount(1, ErrorMessage = "You should specify at least one education")]
        public List<EducationInputModel> Educations { get; set; }
        public EducationInputModel Education { get; set; }

        [BindProperty]
        [MinimumCount(1, ErrorMessage = "You should specify at least one work experience")]
        public List<WorkExperienceInputModel> WorkExperiences { get; set; }
        public WorkExperienceInputModel WorkExperience { get; set; }

        [BindProperty]
        [MinimumCount(1, ErrorMessage = "You should specify at least one skill")]
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
            if (!ModelState.IsValid) return Page();

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

            resume.Educations.Add(new Education
            {
                StartDate = education.StartDate,
                EndDate = education.EndDate,
                Degree = education.Degree,
                FieldOfStudy = _termsManager.GetFieldOfStudy(education.FieldOfStudy),
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

            resume.WorkExperiences.Add(new WorkExperience()
            {
                StartDate = workExperience.StartDate,
                EndDate = workExperience.EndDate,
                Company = company,
                JobTitle = _termsManager.GetJobTitle(workExperience.JobTitle),
                Description = workExperience.Description,
            });
        }

        private void AddSkill(Resume resume, SkillInputModel skill)
        {
            resume.OwnedSkills.Add(new OwnedSkill
            {
                Skill = _termsManager.GetSkill(skill.Skill),
                Years = skill.Years,
            });
        }

        private void AddSeekedJobTitle(Resume resume, JobTitleInputModel jobTitleModel)
        {
            resume.SeekedJobTitles.Add(new SeekedJobTitle
            {
                JobTitle = _termsManager.GetJobTitle(jobTitleModel.JobTitle),
            });
        }
    }
}