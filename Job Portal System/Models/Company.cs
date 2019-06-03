using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.Models
{
    public class Company
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "Employees number")]
        public int? EmployeesNum { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Url)]
        public string Website { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        
        public string Logo { get; set; }

        public bool Approved { get; set; } = false;

        public ICollection<CompanyDepartment> Departments { get; set; }
    }
}