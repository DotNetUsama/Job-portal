using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.Models
{
    public class Skill
    {
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.Text)]
        public string Title { get; set; }
    }
}
