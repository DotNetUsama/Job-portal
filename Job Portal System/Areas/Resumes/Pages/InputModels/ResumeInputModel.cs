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

        [Required]
        [Display(Name = "Moving distance limit (in km)")]
        [Range(0, 60)]
        public uint MovingDistanceLimit { get; set; }

        [Required]
        [Display(Name = "Biography")]
        [DataType(DataType.MultilineText)]
        public string Biography { get; set; }

        [Required]
        [Display(Name = "Seeking for a job")]
        public bool IsSeeking { get; set; } = true;

        public Dictionary<JobType, bool> JobTypes { get; set; }

        [Display(Name = "Make my resume public")]
        public bool IsPublic { get; set; } = true;
    }
}
