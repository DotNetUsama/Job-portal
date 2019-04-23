using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Portal_System.Models
{
    public class SeekedJobTitle
    {
        public string Id { get; set; }

        public Resume Resume { get; set; }
        public JobTitle JobTitle { get; set; }
    }
}
