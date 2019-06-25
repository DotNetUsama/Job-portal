using System.Collections.Generic;

namespace Job_Portal_System.ViewModels.Resumes
{
    public class ResumesIndexViewModel
    {
        public IEnumerable<ResumeGeneralViewModel> Resumes { get; set; }
        public bool IsJobSeeker { get; set; }
        public bool OwnResumeCreated { get; set; }
        public string ActiveTab { get; set; }
    }
}
