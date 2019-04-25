using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Enums;
using Microsoft.AspNetCore.Identity;

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
        
        public int? AwaitingApplicants { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Published at")]
        public DateTime PublishedAt { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Finished at")]
        public DateTime? FinishedAt { get; set; }

        [StringLength(64)]
        public string DecisionTreeFile { get; set; }

        [Display(Name = "Department")]
        public CompanyDepartment CompanyDepartment { get; set; }

        [Display(Name = "Job title")]
        public JobTitle JobTitle { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public Recruiter Recruiter { get; set; }

        public ICollection<EducationQualification> EducationQualifications { get; set; }
        public ICollection<WorkExperienceQualification> WorkExperienceQualifications { get; set; }
        public ICollection<DesiredSkill> DesiredSkills { get; set; }
        public ICollection<JobVacancyJobType> JobTypes { get; set; }
        public ICollection<Applicant> Applicants { get; set; }

    }
}
