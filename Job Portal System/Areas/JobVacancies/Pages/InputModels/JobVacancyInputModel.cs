using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Job_Portal_System.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_System.Areas.JobVacancies.Pages.InputModels
{
    public class JobVacancyInputModel
    {
        [Required]
        [Display(Name = "Job title")]
        public string JobTitle { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Distance limit")]
        [Range(0, 60)]
        public uint DistanceLimit { get; set; } = 0;

        [Required]
        [Display(Name = "Minimum salary")]
        [Range(100, 999999.99)]
        public double MinSalary { get; set; }

        [Required]
        [Display(Name = "Maximum salary")]
        [Range(100, 999999.99)]
        public double MaxSalary { get; set; }

        [Required]
        [Display(Name = "Required hires")]
        [Range(1, 50)]
        public int RequiredHires { get; set; }

        [Required]
        [EnumDataType(typeof(JobVacancyMethod))]
        [Display(Name = "Method")]
        public int Method { get; set; }

        public Dictionary<JobType, bool> JobTypes { get; set; }

        [HiddenInput]
        public string CompanyDepartmentId { get; set; }
    }
}
