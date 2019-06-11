using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Areas.Resumes.Pages.InputModels;
using Job_Portal_System.Data;
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

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
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
                .Include(r => r.Educations).ThenInclude(e => e.School)
                .Include(r => r.WorkExperiences).ThenInclude(w => w.JobTitle)
                .Include(r => r.WorkExperiences).ThenInclude(w => w.Company)
                .Include(r => r.OwnedSkills).ThenInclude(s => s.Skill)
                .Include(r => r.SeekedJobTitles).ThenInclude(j => j.JobTitle)
                .Include(r => r.JobTypes)
                //.SingleOrDefault(r => r.Id == "7a4883a3-5f23-4234-994e-e086e6057307"/*"8de3fe3e-0e6f-4b04-91f1-a4139260da9c"*/);
                .SingleOrDefault(resume => resume.User.UserName == User.Identity.Name);

            if (resumeInDb == null) return Redirect("./Create");

            resumeInDb.SeekedJobTitles.ForEach(AddToSeekedJobTitles);
            SeekedJobTitles.Add(new JobTitleInputModel());
            ResumeInfo = new ResumeInputModel
            {
                IsPublic = resumeInDb.IsPublic,
                IsSeeking = resumeInDb.IsSeeking,
                MinSalary = resumeInDb.MinSalary,
                MovingDistanceLimit = resumeInDb.MovingDistanceLimit,
                Biography = resumeInDb.Biography,
                JobTypes =
                    JobTypeMethods.GetDictionary(resumeInDb.JobTypes.Select(jobType => jobType.JobType).ToList()),
            };
            Resume = resumeInDb;
            return Page();
        }

        public async Task<IActionResult> OnPostSaveChangesAsync()
        {
            var resume = _context.Resumes
                .Include(r => r.Educations)
                .Include(r => r.WorkExperiences)
                .Include(r => r.OwnedSkills)
                .Include(r => r.SeekedJobTitles)
                .Include(r => r.JobTypes)
                .SingleOrDefault(r => r.User.UserName == User.Identity.Name);
            if (resume == null) return BadRequest();
            EducationsEdits.ForEach(EditEducation);
            WorkExpereincesEdits.ForEach(EditWorkExperience);
            SkillsEdits.ForEach(EditSkill);
            PrepareLists();
            Educations.ForEach(education => AddEducation(resume, education));
            WorkExperiences.ForEach(workExperience => AddWorkExperience(resume, workExperience));
            OwnedSkills.ForEach(skill => AddSkill(resume, skill));
            SeekedJobTitles.ForEach(jobTitle => AddSeekedJobTitle(resume, jobTitle));
            AddJobTypes(resume);
            resume.MinSalary = ResumeInfo.MinSalary;
            resume.MovingDistanceLimit = ResumeInfo.MovingDistanceLimit;
            resume.IsPublic = ResumeInfo.IsPublic;
            resume.IsSeeking = ResumeInfo.IsSeeking;
            resume.Biography = ResumeInfo.Biography;
            await _context.SaveChangesAsync();
            return Redirect("./Index");
        }

        public void PrepareLists()
        {
            Educations.RemoveAt(Educations.Count - 1);
            WorkExperiences.RemoveAt(WorkExperiences.Count - 1);
            OwnedSkills.RemoveAt(OwnedSkills.Count - 1);
            SeekedJobTitles.RemoveAt(SeekedJobTitles.Count - 1);
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
            var school = _context.Schools.SingleOrDefault(schoolInDb => schoolInDb.Name == education.School) ??
                new School
                {
                    Name = education.School,
                };

            var fieldOfStudy =
                _context.FieldOfStudies.SingleOrDefault(fieldInDb => fieldInDb.Id == education.FieldOfStudyId) ??
                new FieldOfStudy
                {
                    Title = education.FieldOfStudyName,
                    NormalizedTitle = education.FieldOfStudyName.ToLower(),
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
                _context.Companies.SingleOrDefault(companyInDb => companyInDb.Id == workExperience.CompanyId) ??
                new Company
                {
                    Name = workExperience.Company
                };

            var jobTitle =
                _context.JobTitles.SingleOrDefault(jobTitleInDb => jobTitleInDb.Id == workExperience.JobTitleId) ??
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
                _context.Skills.SingleOrDefault(skillInDb => skillInDb.Id == skillModel.SkillId) ?? 
                new Skill
                {
                    Title = skillModel.Skill,
                };

            resume.OwnedSkills.Add(new OwnedSkill
            {
                Skill = skill,
                Years = skillModel.Years,
            });
        }

        private void AddSeekedJobTitle(Resume resume, JobTitleInputModel jobTitleModel)
        {
            if (resume.SeekedJobTitles.Any(j => j.JobTitle.Id == jobTitleModel.JobTitleId)) return;
            var jobTitle =
                _context.JobTitles.SingleOrDefault(jobTitleInDb => jobTitleInDb.Id == jobTitleModel.JobTitleId) ??
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
                JobTitleId = seekedJobTitle.JobTitle.Id,
            });
        }
    }
}