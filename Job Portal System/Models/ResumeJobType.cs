﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Portal_System.Models
{
    public class ResumeJobType
    {
        public string Id { get; set; }

        [Required]
        public int JobType { get; set; }

        public Resume Resume { get; set; }
    }
}
