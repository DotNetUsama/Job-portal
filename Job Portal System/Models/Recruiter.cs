using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Job_Portal_System.Models
{
    public class Recruiter
    {
        public string Id { get; set; }

        public Company Company { get; set; }
        public User User { get; set; }
        public string CompanyId { get; set; }
        public string UserId { get; set; }
    }
}