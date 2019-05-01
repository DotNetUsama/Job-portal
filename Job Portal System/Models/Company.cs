using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.Models
{
    public class Company
    {
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Display(Name = "Employees number")]
        public int? EmployeesNum { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Url)]
        public string Website { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public bool Approved { get; set; }

        public ICollection<CompanyDepartment> Departments { get; set; }
    }
}