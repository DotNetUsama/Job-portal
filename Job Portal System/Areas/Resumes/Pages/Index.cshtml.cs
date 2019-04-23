using System.Collections.Generic;
using System.Linq;
using Job_Portal_System.Areas.Resumes.Pages.InputModels;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Areas.Resumes.Pages
{
    //[Authorize(Roles = "JobSeeker")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public IndexModel (ApplicationDbContext context,
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public List<EducationInputModel> Educations { get; set; } = new List<EducationInputModel>
        {
            new EducationInputModel()
        };

        [BindProperty] public List<QualificationEditModel> EducationsEdits { get; set; }

        [BindProperty]
        public List<WorkExperienceInputModel> WorkExperiences { get; set; } = new List<WorkExperienceInputModel>
        {
            new WorkExperienceInputModel()
        };

        [BindProperty] public List<QualificationEditModel> WorkExpereincesEdits { get; set; }

        [BindProperty]
        public List<SkillInputModel> OwnedSkills { get; set; } = new List<SkillInputModel>
        {
            new SkillInputModel()
        };

        [BindProperty] public List<SkillEditModel> SkillsEdits { get; set; }

        [BindProperty] public List<JobTitleInputModel> SeekedJobTitles { get; set; } = new List<JobTitleInputModel>();

        [BindProperty]
        public ResumeInputModel ResumeInfo { get; set; }

        public Resume Resume { get; set; }

        public IActionResult OnGet()
        {
            var resumeInDb = _context.Resumes
                .Include(r => r.Educations).ThenInclude(e => e.FieldOfStudy)
                .Include(r => r.Educations).ThenInclude(e => e.School).ThenInclude(s => s.City)
                .Include(r => r.WorkExperiences).ThenInclude(w => w.JobTitle)
                .Include(r => r.WorkExperiences).ThenInclude(w => w.Company)
                .Include(r => r.OwnedSkills).ThenInclude(s => s.Skill)
                .Include(r => r.SeekedJobTitles).ThenInclude(j => j.JobTitle)
                .Include(r => r.JobTypes)
                .SingleOrDefault(r => r.Id == "8de3fe3e-0e6f-4b04-91f1-a4139260da9c");
                //.SingleOrDefault(resume => resume.User.UserName == User.Identity.Name);

            if (resumeInDb == null) return Redirect("./Create");
            
            //resumeInDb.Educations.ForEach(AddToEducationsEdits);
            //resumeInDb.WorkExperiences.ForEach(AddToWorkExperiencesEdits);
            //resumeInDb.OwnedSkills.ForEach(AddToSkillsEdits);
            resumeInDb.SeekedJobTitles.ForEach(AddToSeekedJobTitles);
            SeekedJobTitles.Add(new JobTitleInputModel());
            ResumeInfo = new ResumeInputModel
            {
                IsPublic = resumeInDb.IsPublic,
                MinSalary = resumeInDb.MinSalary,
                JobTypes =
                    JobTypeMethods.GetDictionary(resumeInDb.JobTypes.Select(jobType => jobType.JobType).ToList()),
            };
            Resume = resumeInDb;
            return Page();
        }

        private void AddToEducationsEdits(Education education)
        {
            if (!education.EndDate.HasValue)
            {
                EducationsEdits.Add(new QualificationEditModel
                {
                    Id = education.Id,
                    EndDate = null,
                });
            }
        }

        private void AddToWorkExperiencesEdits(WorkExperience workExperience)
        {
            if (!workExperience.EndDate.HasValue)
            {
                WorkExpereincesEdits.Add(new QualificationEditModel
                {
                    Id = workExperience.Id,
                    EndDate = null,
                });
            }
        }

        private void AddToSkillsEdits(OwnedSkill skill)
        {
            SkillsEdits.Add(new SkillEditModel
            {
                Id = skill.Id,
                Years = skill.Years,
            });
        }

        private void AddToSeekedJobTitles(SeekedJobTitle seekedJobTitle)
        {
            SeekedJobTitles.Add(new JobTitleInputModel
            {
                JobTitle = seekedJobTitle.JobTitle.Title,
                JobTitleId = seekedJobTitle.JobTitle.Id,
            });
        }
    }
}