using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Portal_System.Models
{
    public class WorkExperience
    {
        public string Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public Resume Resume { get; set; }
        public JobTitle JobTitle { get; set; }
        public Company Company { get; set; }
    }
}
