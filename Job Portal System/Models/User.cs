using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Job_Portal_System.Models
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(25)]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(25)]
        public string LastName { get; set; }

        [Required]
        public byte Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Text)]
        [StringLength(255)]
        [Display(Name = "Detailed address")]
        public string DetailedAddress { get; set; }

        public City City { get; set; }

        public string CityId { get; set; }
    }
}
