using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Portal_System.Models
{
    public class School
    {
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        public string CityId { get; set; }
        public City City { get; set; }
    }
}
