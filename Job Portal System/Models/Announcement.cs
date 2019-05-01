using System;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.Models
{
    public class Announcement
    {
        public string Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(255)]
        public string Headline { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Content { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Image { get; set; }
        
        [Display(Name = "Created at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Updated at")]
        public DateTime? UpdatedAt { get; set; }
        
        public User Author { get; set; }
        public string UserId { get; set; }
    }
}
