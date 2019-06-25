using System;
using System.Collections.Generic;
using Job_Portal_System.Enums;

namespace Job_Portal_System.ViewModels.Applicants
{
    public class ApplicantForRecruiterFullViewModel
    {
        public string Id { get; set; }
        public string OwnerName { get; set; }
        public int WorksCount { get; set; }
        public int EducationsCount { get; set; }
        public int SkillsCount { get; set; }
        public string Location { get; set; }
        public ApplicantStatus Status { get; set; }
        public IEnumerable<string> Skills { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
