using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Data;

namespace Job_Portal_System.Models
{
    public class CompanyDepartment
    {
        public string Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(255)]
        [Display(Name = "Detailed address")]
        public string DetailedAddress { get; set; }

        public City City { get; set; }
        public Company Company { get; set; }
        public string CompanyId { get; set; }
    }
}
