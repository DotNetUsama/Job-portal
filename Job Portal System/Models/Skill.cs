using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.Models
{
    public class Skill
    {
        public long Id { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.Text)]
        public string NormalizedTitle { get; set; }
    }
}
