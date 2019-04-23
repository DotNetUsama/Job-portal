using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Job_Portal_System.Models
{
    public class Resume
    {
        public string Id { get; set; }

        [Required]
        [Range(100, double.MaxValue)]
        public double MinSalary { get; set; }
        
        public bool IsPublic { get; set; } = true;

        public User User { get; set; }
        public string UserId { get; set; }
        public JobSeeker JobSeeker { get; set; }

        public List<Education> Educations { get; set; }
        public List<WorkExperience> WorkExperiences { get; set; }
        public List<OwnedSkill> OwnedSkills { get; set; }
        public List<ResumeJobType> JobTypes { get; set; }
        public List<SeekedJobTitle> SeekedJobTitles { get; set; }
    }
}
