﻿using System.Collections.Generic;
using Job_Portal_System.Models;

namespace Job_Portal_System.ViewModels
{
    public class JobVacanciesSearchResult : AbstractSearchResultViewModel
    {
        public IEnumerable<JobVacancy> JobVacancies { get; set; }
    }
}
