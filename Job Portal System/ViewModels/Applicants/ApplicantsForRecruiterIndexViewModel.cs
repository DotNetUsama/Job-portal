using System;
using System.Collections.Generic;
using Job_Portal_System.Enums;

namespace Job_Portal_System.ViewModels.Applicants
{
    public class ApplicantsForRecruiterIndexViewModel
    {
        public string JobVacancyId { get; set; }
        public string JobVacancyTitle { get; set; }
        public int ApplicantsCount { get; set; }
        public string JobTitle { get; set; }
        public bool IsRemote { get; set; }
        public string Location { get; set; }
        public JobVacancyStatus Status { get; set; }
        public IEnumerable<ApplicantForRecruiterGeneralViewModel> Applicants { get; set; }
    }
}
