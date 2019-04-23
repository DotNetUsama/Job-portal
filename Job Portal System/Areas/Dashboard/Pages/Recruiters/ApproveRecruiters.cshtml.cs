using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Areas.Dashboard.Pages.Recruiters
{
    [Authorize(Roles = "Administrator")]
    public class ApproveRecruitersModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ApproveRecruitersModel(ApplicationDbContext context,
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Recruiter> Recruiters { get; set; }

        public async Task OnGetAsync()
        {
            Recruiters = new List<Recruiter>();
            var allRecruiters = _context.Recruiters
                .Include(recruiter => recruiter.User)
                .Include(recruiter => recruiter.Company)
                .ToList();
            foreach (var recruiter in allRecruiters)
            {
                if (await _userManager.IsInRoleAsync(recruiter.User, "PendingRecruiter"))
                {
                    Recruiters.Add(recruiter);
                }
            }
        }
    }
}