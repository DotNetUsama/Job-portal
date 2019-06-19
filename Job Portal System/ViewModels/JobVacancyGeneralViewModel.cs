using System;
using System.Collections.Generic;

namespace Job_Portal_System.ViewModels
{
    public class JobVacancyGeneralViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string JobTitle { get; set; }
        public string Company { get; set; }
        public double MinSalary { get; set; }
        public double MaxSalary { get; set; }
        public bool IsRemote { get; set; }
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<string> DesiredSkills { get; set; }
    }
}
