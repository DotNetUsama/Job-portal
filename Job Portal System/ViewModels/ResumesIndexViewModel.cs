using System.Collections.Generic;
using Job_Portal_System.Models;

namespace Job_Portal_System.ViewModels
{
    public class ResumesIndexViewModel
    {
        public IEnumerable<ResumeGeneralViewModel> Resumes { get; set; }
        public bool IsJobSeeker { get; set; }
        public bool OwnResumeCreated { get; set; }
        public string ActiveTab { get; set; }
    }
}
