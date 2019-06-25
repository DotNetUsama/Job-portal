using System.Collections.Generic;
using Job_Portal_System.Models;

namespace Job_Portal_System.ViewModels.Resumes
{
    public class ResumesSearchResult : AbstractSearchResultViewModel
    {
        public IEnumerable<Resume> Resumes { get; set; }
    }
}
