using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Job_Portal_System.Enums;

namespace Job_Portal_System.Areas.Resumes.Pages.InputModels
{
    public class ResumeInputModel
    {
        [Required]
        [Display(Name = "Minimum salary")]
        [Range(100, 999999.99)]
        public double MinSalary { get; set; }

        public Dictionary<JobType, bool> JobTypes { get; set; }

        [Display(Name = "Make my resume public")]
        public bool IsPublic { get; set; } = true;
    }
}
