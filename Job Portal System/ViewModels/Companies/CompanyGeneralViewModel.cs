using System.Collections.Generic;

namespace Job_Portal_System.ViewModels.Companies
{
    public class CompanyGeneralViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public int? FoundedYear { get; set; }
        public int? EmployeesNum { get; set; }
        public int DepartmentsNum { get; set; }
        public string Type { get; set; }
        public int OpenedJobsNum { get; set; }
    }
}
