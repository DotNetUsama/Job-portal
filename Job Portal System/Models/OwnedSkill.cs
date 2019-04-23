using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Portal_System.Models
{
    public class OwnedSkill
    {
        public string Id { get; set; }

        [Required]
        [Range(1, 25)]
        public int Years { get; set; }

        public Resume Resume { get; set; }
        public Skill Skill { get; set; }
    }
}
