using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Handlers;
using Job_Portal_System.Models;
using Job_Portal_System.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Areas.Dashboard.Pages.Recruiters
{
    [Authorize(Roles = "Administrator")]
    public class ApproveModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SignalRHub> _hubContext;
        private readonly UserManager<User> _userManager;

        public ApproveModel(ApplicationDbContext context, 
            IHubContext<SignalRHub> hubContext, UserManager<User> userManager)
        {
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        [BindProperty]
        public Recruiter Recruiter { get; set; }

        public async Task<IActionResult> OnGetAsync(string recruiterId)
        {
            var checkResult = await Check(recruiterId);
            return checkResult ?? Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(string recruiterId)
        {
            var checkResult = await Check(recruiterId);
            if (checkResult != null)
                return checkResult;
            await AsyncHandler.ApproveRecruiterAsync(
                context: _context, 
                hubContext: _hubContext, 
                userManager: _userManager, 
                recruiter: Recruiter);
            return Redirect("Dashboard");
        }

        public async Task<IActionResult> OnPostRejectAsync(string recruiterId)
        {
            var checkResult = await Check(recruiterId);
            if (checkResult != null)
                return checkResult;
            await AsyncHandler.RejectRecruiterAsync(
                context: _context,
                hubContext: _hubContext,
                userManager: _userManager,
                recruiter: Recruiter);
            return Redirect("Dashboard");
        }

        private async Task<IActionResult> Check(string recruiterId)
        {
            if (recruiterId == null)
            {
                return NotFound();
            }

            Recruiter = await _context.Recruiters
                .Include(recruiter => recruiter.User)
                .Include(recruiter => recruiter.Company)
                .FirstOrDefaultAsync(recruiter => recruiter.Id == recruiterId);

            if (Recruiter == null)
            {
                return NotFound();
            }

            if (!await _userManager.IsInRoleAsync(Recruiter.User, "PendingRecruiter"))
            {
                return BadRequest();
            }

            return null;
        }
    }
}