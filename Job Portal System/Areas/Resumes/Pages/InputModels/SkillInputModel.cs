using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_System.Areas.Resumes.Pages.InputModels
{
    public class SkillInputModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Skill")]
        public string Skill { get; set; }

        [Required]
        [Display(Name = "Years")]
        [Range(1, 25)]
        public int Years { get; set; }
    }
}
