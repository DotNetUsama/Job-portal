﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.Models
{
    public class JobTitle
    {
        public long Id { get; set; }
        
        [Required]
        [StringLength(70)]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        public ICollection<JobTitleSimilarity> Similarities { get; set; }
    }
}