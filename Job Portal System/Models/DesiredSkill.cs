using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Portal_System.Models
{
    public class DesiredSkill
    {
        public string Id { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        [Range(1, 25)]
        public int MinimumYears { get; set; }

        public double? Min { get; set; }
        public double? Range { get; set; }

        public JobVacancy JobVacancy { get; set; }
        public Skill Skill { get; set; }
    }
}
