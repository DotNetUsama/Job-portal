using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_System.Areas.Resumes.Pages.InputModels
{
    public class QualificationEditModel
    {
        [Required]
        [HiddenInput]
        public string Id { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "End date")]
        public DateTime? EndDate { get; set; }
    }
}
