using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Job_Portal_System.Models
{
    public class City
    {
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        
        public double? Longitude { get; set; }
        
        public double? Latitude { get; set; }
    }
}
