namespace Job_Portal_System.ViewModels.JobVacancies
{
    public class JobVacancyActionsForRecruiterViewModel
    {
        public bool CanClose { get; set; }
        public bool CanDelete { get; set; }
        public bool CanFinalDecide { get; set; }
        public string JobVacancyId { get; set; }
    }
}
