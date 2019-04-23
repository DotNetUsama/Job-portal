using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Portal_System.Models
{
    public class EducationQualification
    {
        public string Id { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        [Range(1, 25)]
        public int MinimumYears { get; set; }

        [Required]
        public int Degree { get; set; }

        public double? Min { get; set; }
        public double? Range { get; set; }

        public JobVacancy JobVacancy { get; set; }
        public FieldOfStudy FieldOfStudy { get; set; }
    }
}
