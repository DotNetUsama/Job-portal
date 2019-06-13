using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_System.Areas.Companies.InputModels
{
    public class DepartmentEditModel
    {
        [Required]
        [HiddenInput]
        public string Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(255)]
        [Display(Name = "Address")]
        public string DetailedAddress { get; set; }
    }
}
