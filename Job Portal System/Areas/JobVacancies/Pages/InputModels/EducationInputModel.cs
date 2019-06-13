using System.ComponentModel.DataAnnotations;
using Job_Portal_System.Enums;

namespace Job_Portal_System.Areas.JobVacancies.Pages.InputModels
{
    public class EducationInputModel
    {
        [Required]
        [EnumDataType(typeof(EducationDegree))]
        [Display(Name = "Degree")]
        public int Degree { get; set; }

        [Required]
        [EnumDataType(typeof(QualificationType))]
        [Display(Name = "Type")]
        public int Type { get; set; }

        [Required]
        [Display(Name = "Minimum years")]
        [Range(1, 10)]
        public int MinimumYears { get; set; }

        [Required]
        [Display(Name = "Field of study")]
        public string FieldOfStudy { get; set; }
    }
}
