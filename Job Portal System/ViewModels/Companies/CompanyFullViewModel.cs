using System.Collections.Generic;
using Job_Portal_System.ViewModels.JobVacancies;

namespace Job_Portal_System.ViewModels.Companies
{
    public class CompanyFullViewModel
    {
        public bool CanEdit { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Description { get; set; }
        public int? FoundedYear { get; set; }
        public int? EmployeesNum { get; set; }
        public string Type { get; set; }
        public string Website { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public IEnumerable<DepartmentViewModel> Departments { get; set; }
        public IEnumerable<JobVacancyGeneralViewModel> JobVacancies { get; set; }
    }
}
