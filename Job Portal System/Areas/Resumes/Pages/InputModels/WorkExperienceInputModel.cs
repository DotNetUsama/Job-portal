using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_System.Areas.Resumes.Pages.InputModels
{
    public class WorkExperienceInputModel
    {
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End date")]
        public DateTime? EndDate { get; set; }

        [Required]
        [HiddenInput]
        [Display(Name = "Job title")]
        public string JobTitle { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [HiddenInput]
        public long JobTitleId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Company")]
        public string Company { get; set; }

        [HiddenInput]
        public string CompanyId { get; set; }
    }
}
