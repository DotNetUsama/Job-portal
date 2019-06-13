using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.Areas.Resumes.Pages.InputModels
{
    public class JobTitleInputModel
    {
        [Required]
        [Display(Name = "Job title")]
        public string JobTitle { get; set; }
    }
}
