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
        public string Title { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        public double MinSalary { get; set; }

        [Required]
        public double MaxSalary { get; set; }

        [Required]
        public int RequiredHires { get; set; }

        [Required]
        public int Method { get; set; }

        public int Status { get; set; } = (int)JobVacancyStatus.Open;
        
        public int? AwaitingApplicants { get; set; }

        [DataType(DataType.Date)]
        public DateTime PublishedAt { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime? FinishedAt { get; set; }

        [StringLength(64)]
        public string DecisionTreeFile { get; set; }

        public City City { get; set; }
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
