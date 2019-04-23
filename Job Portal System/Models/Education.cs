using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Portal_System.Models
{
    public class Education
    {
        public string Id { get; set; }

        [Required]
        public int Degree { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public Resume Resume { get; set; }
        public FieldOfStudy FieldOfStudy { get; set; }
        public School School { get; set; }
    }
}
