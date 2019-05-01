using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.Models
{
    public class ResumeJobType
    {
        public string Id { get; set; }

        [Required]
        public int JobType { get; set; }

        public Resume Resume { get; set; }
    }
}
