using System.Collections.Generic;

namespace Job_Portal_System.ViewModels.JobVacancies
{
    public class JobVacanciesIndexViewModel
    {
        public IEnumerable<JobVacancyGeneralViewModel> JobVacancies { get; set; }
        public bool IsRecruiter { get; set; }
        public string ActiveTab { get; set; }
    }
}
