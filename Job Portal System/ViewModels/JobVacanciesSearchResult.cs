using System.Collections.Generic;
using Job_Portal_System.Models;

namespace Job_Portal_System.ViewModels
{
    public class JobVacanciesSearchResult
    {
        public IEnumerable<JobVacancy> JobVacancies { get; set; }
        public string Query { get; set; }
    }
}
