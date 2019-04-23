using System;
using System.ComponentModel.DataAnnotations;
using Job_Portal_System.Enums;
using Microsoft.AspNetCore.Mvc;

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
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End date")]
        public DateTime? EndDate { get; set; }

        [Required]
        [HiddenInput]
        [Display(Name = "Field of study")]
        public string FieldOfStudyName { get; set; }

        [Required]
        [HiddenInput]
        public long FieldOfStudyId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "City")]
        public string SchoolCity { get; set; }

        [HiddenInput]
        public string SchoolCityId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "School")]
        public string School { get; set; }

        [HiddenInput]
        public string SchoolId { get; set; }
    }
}
