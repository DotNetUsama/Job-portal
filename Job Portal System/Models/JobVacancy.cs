using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Job_Portal_System.Enums;

namespace Job_Portal_System.Models
{
    public class JobVacancy
    {
        public string Id { get; set; }

        [Required]
        [StringLength(128)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Minimum salary")]
        public double MinSalary { get; set; }

        [Required]
        [Display(Name = "Maximum salary")]
        public double MaxSalary { get; set; }

        [Required]
        [Display(Name = "Required hires")]
        public int RequiredHires { get; set; }

        [Required]
        [Display(Name = "Method")]
        public int Method { get; set; }

        public int Status { get; set; } = (int)JobVacancyStatus.Open;

        public int AwaitingApplicants { get; set; } = 0;

        public double Min { get; set; }
        public double Range { get; set; } = 1;

        [DataType(DataType.Date)]
        [Display(Name = "Published at")]
        public DateTime PublishedAt { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Finished at")]
        public DateTime? FinishedAt { get; set; }

        [Display(Name = "Department")]
        public CompanyDepartment CompanyDepartment { get; set; }

        [Display(Name = "Job title")]
        public JobTitle JobTitle { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public Recruiter Recruiter { get; set; }

        public List<EducationQualification> EducationQualifications { get; set; }
        public List<WorkExperienceQualification> WorkExperienceQualifications { get; set; }
        public List<DesiredSkill> DesiredSkills { get; set; }
        public List<JobVacancyJobType> JobTypes { get; set; }
        public List<Applicant> Applicants { get; set; }

        public void SetMinAndRange(double[] minRange)
        {
            Min = minRange[0];
            Range = minRange[1];
        }
    }
}
