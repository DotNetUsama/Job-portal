using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.Enums
{
    public enum JobVacancyMethod
    {
        [Display(Name = "Submission")]
        [Description("Submission Description")]
        Submission,

        [Display(Name = "Recommendation")]
        [Description("Recommendation Description")]
        Recommendation,
    }
}
