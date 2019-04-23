using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
