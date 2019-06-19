using System.Collections.Generic;
using Job_Portal_System.Models;

namespace Job_Portal_System.ViewModels
{
    public class JobVacanciesIndexViewModel
    {
        public IEnumerable<JobVacancyGeneralViewModel> JobVacancies { get; set; }
        public bool IsRecruiter { get; set; }
        public string ActiveTab { get; set; }
    }
}
