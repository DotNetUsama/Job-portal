using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.Enums
{
    public enum QualificationType
    {
        [Display(Name = "Preferred")] Preferred,
        [Display(Name = "Required")] Required,
    }
}
