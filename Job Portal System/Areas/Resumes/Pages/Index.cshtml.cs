using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Areas.Resumes.Pages.InputModels;
using Job_Portal_System.Data;
using Job_Portal_System.Dependencies;
using Job_Portal_System.Enums;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Areas.Resumes.Pages
{
    [Authorize(Roles = "JobSeeker")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ITermsManager _termsManager;

        public IndexModel(ApplicationDbContext context,
            ITermsManager termsManager)
        {
            _context = context;
            _termsManager = termsManager;
        }

        [BindProperty]
        public List<EducationInputModel> Educations { get; set; }
        public EducationInputModel Education { get; set; }

        [BindProperty]
        public List<QualificationEditModel> EducationsEdits { get; set; }

        [BindProperty]
        public List<WorkExperienceInputModel> WorkExperiences { get; set; }
        public WorkExperienceInputModel WorkExperience { get; set; }

        [BindProperty]
        public List<QualificationEditModel> WorkExperiencesEdits { get; set; }

        [BindProperty]
        public List<SkillInputModel> OwnedSkills { get; set; }
        public SkillInputModel OwnedSkill { get; set; }

        [BindProperty]
        public List<SkillEditModel> SkillsEdits { get; set; }

        [BindProperty] public List<JobTitleInputModel> SeekedJobTitles { get; set; } = new List<JobTitleInputModel>();
        public JobTitleInputModel SeekedJobTitle { get; set; }

        [BindProperty]
        public ResumeInputModel ResumeInfo { get; set; }

        public Resume Resume { get; set; }

        public IActionResult OnGet()
        {
            GetResumeFromDb();

            if (Resume == null) return Redirect("./Create");

            Resume.SeekedJobTitles.ForEach(AddToSeekedJobTitles);
            SeekedJobTitles.Add(new JobTitleInputModel());
            ResumeInfo = new ResumeInputModel
            {
                IsPublic = Resume.IsPublic,
                IsSeeking = Resume.IsSeeking,
                MinSalary = Resume.MinSalary,
                MovingDistanceLimit = Resume.MovingDistanceLimit,
                Biography = Resume.Biography,
                JobTypes =
                    JobTypeMethods.GetDictionary(Resume.JobTypes.Select(jobType => jobType.JobType).ToList()),
            };
            return Page();
        }

        public async Task<IActionResult> OnPostSaveChangesAsync()
        {
            if (!ModelState.IsValid)
            {
                GetResumeFromDb();
                return Page();
            }

            var resume = _context.Resumes
                .Include(r => r.Educations)
                .Include(r => r.WorkExperiences)
                .Include(r => r.OwnedSkills)
                .Include(r => r.SeekedJobTitles).ThenInclude(s => s.JobTitle)
                .Include(r => r.JobTypes)
                .SingleOrDefault(r => r.User.UserName == User.Identity.Name);
            if (resume == null) return BadRequest();
            EducationsEdits.ForEach(EditEducation);
            WorkExperiencesEdits.ForEach(EditWorkExperience);
            SkillsEdits.ForEach(EditSkill);
            Educations?.ForEach(education => AddEducation(resume, education));
            WorkExperiences?.ForEach(workExperience => AddWorkExperience(resume, workExperience));
            OwnedSkills?.ForEach(skill => AddSkill(resume, skill));
            UpdateSeekedJobTitles(resume);
            AddJobTypes(resume);
            resume.MinSalary = ResumeInfo.MinSalary;
            resume.MovingDistanceLimit = ResumeInfo.MovingDistanceLimit;
            resume.IsPublic = ResumeInfo.IsPublic;
            resume.IsSeeking = ResumeInfo.IsSeeking;
            resume.Biography = ResumeInfo.Biography;
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

        private void UpdateSeekedJobTitles(Resume resume)
        {
            if (SeekedJobTitles == null) return;
            resume.SeekedJobTitles
                .RemoveAll(j => SeekedJobTitles
                    .All(jm => jm.JobTitle.ToLower() != j.JobTitle.NormalizedTitle));
            SeekedJobTitles.ForEach(jobTitle => AddSeekedJobTitle(resume, jobTitle));
        }

        private void AddSeekedJobTitle(Resume resume, JobTitleInputModel jobTitleModel)
        {
            if (resume.SeekedJobTitles.Any(j => j.JobTitle.NormalizedTitle == jobTitleModel.JobTitle.ToLower()))
            {
                return;
            }
            resume.SeekedJobTitles.Add(new SeekedJobTitle
            {
                JobTitle = _termsManager.GetJobTitle(jobTitleModel.JobTitle),
            });
        }

        private void EditEducation(QualificationEditModel educationEdit)
        {
            if (!educationEdit.EndDate.HasValue) return;
            var education = _context.Educations.SingleOrDefault(e => e.Id == educationEdit.Id);
            if (education != null)
            {
                education.EndDate = educationEdit.EndDate;
            }
        }

        private void EditWorkExperience(QualificationEditModel workExperienceEdit)
        {
            if (!workExperienceEdit.EndDate.HasValue) return;
            var workExperience = _context.WorkExperiences.SingleOrDefault(w => w.Id == workExperienceEdit.Id);
            if (workExperience != null)
            {
                workExperience.EndDate = workExperienceEdit.EndDate;
            }
        }

        private void EditSkill(SkillEditModel skillEdit)
        {
            var skill = _context.OwnedSkills.SingleOrDefault(s => s.Id == skillEdit.Id);
            if (skill != null)
            {
                skill.Years = skill.Years;
            }
        }

        private void AddToSeekedJobTitles(SeekedJobTitle seekedJobTitle)
        {
            SeekedJobTitles.Add(new JobTitleInputModel
            {
                JobTitle = seekedJobTitle.JobTitle.Title,
            });
        }

        private void GetResumeFromDb()
        {
            Resume = _context.Resumes
                .Include(r => r.Educations).ThenInclude(e => e.FieldOfStudy)
                .Include(r => r.Educations).ThenInclude(e => e.School)
                .Include(r => r.WorkExperiences).ThenInclude(w => w.JobTitle)
                .Include(r => r.WorkExperiences).ThenInclude(w => w.Company)
                .Include(r => r.OwnedSkills).ThenInclude(s => s.Skill)
                .Include(r => r.SeekedJobTitles).ThenInclude(j => j.JobTitle)
                .Include(r => r.JobTypes)
                .SingleOrDefault(resume => resume.User.UserName == User.Identity.Name);
        }
    }
}