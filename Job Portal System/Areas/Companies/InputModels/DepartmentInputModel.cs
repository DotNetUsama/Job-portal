using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.Areas.Companies.InputModels
{
    public class DepartmentInputModel
    {
        [Required]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(255)]
        [Display(Name = "Detailed address")]
        public string DetailedAddress { get; set; }
    }
}
