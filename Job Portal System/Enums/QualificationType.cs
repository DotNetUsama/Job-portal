﻿using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.Enums
{
    public enum QualificationType
    {
        [Display(Name = "Preferred")] Preferred,
        [Display(Name = "Required")] Required,
    }

    public static class QualificationTypeMethods
    {
        public static string GetCssClass(this QualificationType type)
        {
            switch (type)
            {
                case QualificationType.Preferred: return "text-success";
                case QualificationType.Required: return "text-danger";
                default: return null;
            }
        }

        public static double GetWeight(this QualificationType qualificationType)
        {
            switch (qualificationType)
            {
                case QualificationType.Preferred: return 1;
                case QualificationType.Required: return 2;
                default: return 0;
            }
        }
    }
}
