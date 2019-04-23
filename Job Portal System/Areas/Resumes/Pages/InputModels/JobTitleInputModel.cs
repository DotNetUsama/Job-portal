using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_System.Areas.Resumes.Pages.InputModels
{
    public class JobTitleInputModel
    {
        [Required]
        [HiddenInput]
        [Display(Name = "Job title")]
        public string JobTitle { get; set; }

        [HiddenInput]
        public long JobTitleId { get; set; }
    }
}
