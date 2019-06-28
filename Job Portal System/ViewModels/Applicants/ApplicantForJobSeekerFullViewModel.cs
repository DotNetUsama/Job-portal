using System;
using Job_Portal_System.Enums;
using Job_Portal_System.ViewModels.JobVacancies;

namespace Job_Portal_System.ViewModels.Applicants
{
    public class ApplicantForJobSeekerFullViewModel
    {
        public string Id { get; set; }
        public ApplicantStatus Status { get; set; }
        public DateTime SubmittedAt { get; set; }
        public JobVacancyFullViewModel JobVacancy { get; set; }
    }
}
