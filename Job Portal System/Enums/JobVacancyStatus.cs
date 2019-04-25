
namespace Job_Portal_System.Enums
{
    public enum JobVacancyStatus
    {
        Open,
        Closed,
        Finished,
    }

    public static class JobVacancyStatusMethods
    {
        public static string GetCssClass(this JobVacancyStatus status)
        {
            switch (status)
            {
                case JobVacancyStatus.Open: return "badge badge-info text-white";
                case JobVacancyStatus.Closed: return "badge badge-danger text-white";
                case JobVacancyStatus.Finished: return "badge badge-secondary text-white";
                default: return null;
            }
        }
    }
}
