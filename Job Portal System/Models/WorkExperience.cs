﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Job_Portal_System.Utilities.RankingSystem.Interfaces;

namespace Job_Portal_System.Models
{
    public class WorkExperience : INumerable
    {
        public string Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [DataType(DataType.MultilineText)]
        [Column(TypeName = "text")]
        public string Description { get; set; }

        public Resume Resume { get; set; }
        public string ResumeId { get; set; }

        public JobTitle JobTitle { get; set; }
        public long JobTitleId { get; set; }

        public Company Company { get; set; }
        public string CompanyId { get; set; }

        public long ToNumber()
        {
            return JobTitle.JobTitleSynsetId;
        }
    }
}
