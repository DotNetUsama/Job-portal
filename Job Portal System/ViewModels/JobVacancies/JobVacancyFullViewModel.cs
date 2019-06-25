using System.Collections.Generic;
using Job_Portal_System.Models;
using Job_Portal_System.ViewModels.Companies;

namespace Job_Portal_System.ViewModels.JobVacancies
{
    public class JobVacancyFullViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string JobTitle { get; set; }
        public double MinSalary { get; set; }
        public double MaxSalary { get; set; }
        public bool IsRemote { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public IEnumerable<int> JobTypes { get; set; }
        public CompanyFullViewModel Company { get; set; }
        public IEnumerable<DesiredSkill> DesiredSkills { get; set; }
        public IEnumerable<WorkExperienceQualification> WorkExperienceQualifications { get; set; }
        public IEnumerable<EducationQualification> EducationQualifications { get; set; }
        public IEnumerable<JobVacancyGeneralViewModel> RelatedJobVacancies { get; set; }
    }
}
