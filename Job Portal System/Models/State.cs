using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.Models
{
    public class State
    {
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
