﻿
namespace Job_Portal_System.Enums
{
    public enum JobVacancyStatus
    {
        Open,
        Closed,
        InAction,
        Finished,
    }

    public static class JobVacancyStatusMethods
    {
        public static string GetCssClass(this JobVacancyStatus status)
        {
            switch (status)
            {
                case JobVacancyStatus.Open: return "badge badge-success text-white";
                case JobVacancyStatus.Closed: return "badge badge-danger text-white";
                case JobVacancyStatus.InAction: return "badge badge-success text-white";
                case JobVacancyStatus.Finished: return "badge badge-secondary text-white";
                default: return null;
            }
        }
    }
}