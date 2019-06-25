using System.Collections.Generic;
using Job_Portal_System.Models;
using Job_Portal_System.ViewModels.Companies;
using Job_Portal_System.ViewModels.JobVacancies;

namespace Job_Portal_System.ViewModels.Resumes
{
    public class ResumeFullViewModel
    {
        public Resume Resume { get; set; }
        public bool IsOwner { get; set; }
    }
}
