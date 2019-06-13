using System;
using System.ComponentModel.DataAnnotations;
using Job_Portal_System.Enums;

namespace Job_Portal_System.Areas.Resumes.Pages.InputModels
{
    public class EducationInputModel
    {
        [Required]
        [EnumDataType(typeof(EducationDegree))]
        [Display(Name = "Degree")]
        public int Degree { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End date")]
        public DateTime? EndDate { get; set; }

        [Required]
        [Display(Name = "Field of study")]
        public string FieldOfStudy { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "School")]
        public string School { get; set; }
    }
}
