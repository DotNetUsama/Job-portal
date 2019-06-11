using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

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
        [Column(TypeName = "text")]
        public string Description { get; set; }

        [HiddenInput]
        [DataType(DataType.ImageUrl)]
        public string Logo { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public string Type { get; set; }

        public int? FoundedYear { get; set; }

        public bool Approved { get; set; } = false;

        public ICollection<CompanyDepartment> Departments { get; set; }
    }
}