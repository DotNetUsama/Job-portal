using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.Models
{
    public class JobVacancyJobType
    {
        public string Id { get; set; }

        [Required]
        public int JobType { get; set; }

        public JobVacancy JobVacancy { get; set; }
        public string JobVacancyId { get; set; }
    }
}
