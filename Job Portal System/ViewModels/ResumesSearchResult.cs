using System.Collections.Generic;
using Job_Portal_System.Models;

namespace Job_Portal_System.ViewModels
{
    public class ResumesSearchResult
    {
        public IEnumerable<Resume> Resumes { get; set; }
        public string Query { get; set; }
    }
}
