using System.ComponentModel.DataAnnotations;
using Job_Portal_System.Enums;

namespace Job_Portal_System.Areas.JobVacancies.Pages.InputModels
{
    public class SkillInputModel
    {
        [Required]
        [EnumDataType(typeof(QualificationType))]
        [Display(Name = "Type")]
        public int Type { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Skill")]
        public string Skill { get; set; }

        [Required]
        [Display(Name = "Minimum years")]
        [Range(1, 25)]
        public int MinimumYears { get; set; }
    }
}
