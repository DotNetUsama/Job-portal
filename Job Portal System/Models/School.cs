using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.Models
{
    public class School
    {
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.Text)]
        public string Name { get; set; }
    }
}
