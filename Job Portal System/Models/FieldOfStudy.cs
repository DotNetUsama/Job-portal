using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.Models
{
    public class FieldOfStudy
    {
        public long Id { get; set; }

        [Required]
        [StringLength(70)]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        [Required]
        [StringLength(70)]
        [DataType(DataType.Text)]
        public string NormalizedTitle { get; set; }
    }
}
