using System;
using System.Collections.Generic;

namespace Job_Portal_System.ViewModels.Resumes
{
    public class ResumeGeneralViewModel
    {
        public string Id { get; set; }
        public string OwnerName { get; set; }
        public IEnumerable<string> SeekedJobTitles { get; set; }
        public int WorksCount { get; set; }
        public int EducationsCount { get; set; }
        public int SkillsCount { get; set; }
        public string Location { get; set; }
        public bool IsModified { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
