using System.Collections.Generic;

namespace Job_Portal_System.ViewModels.Companies
{
    public class CompaniesIndexViewModel
    {
        public IEnumerable<CompanyGeneralViewModel> Companies { get; set; }
        public bool IsRecruiter { get; set; }
    }
}
