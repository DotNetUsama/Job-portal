using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Enums;
using Microsoft.AspNetCore.Identity;

namespace Job_Portal_System.Models
{
    public class Applicant
    {
        public string Id { get; set; }

        public int Status { get; set; } = (int) ApplicantStatus.WaitingRecruiterDecision;
        
        [Display(Name = "Submitted at")]
        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        public User Recruiter { get; set; }
        public User JobSeeker { get; set; }

        public JobVacancy JobVacancy { get; set; }
        public Resume Resume { get; set; }
    }
}
