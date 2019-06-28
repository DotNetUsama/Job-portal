using System.Security.Claims;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.ViewComponents
{
    public class ApplicantActionsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ApplicantActionsViewComponent(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string applicantId, ClaimsPrincipal user)
        {
            var userId = _userManager.GetUserId(user);
            var applicant = await _context.Applicants.FirstOrDefaultAsync(j => j.Id == applicantId);

            if (applicant == null) return View();

            if (user.IsInRole("JobSeeker") && applicant.JobSeekerId == userId)
            {
                return View("ForJobSeeker", applicant);
            }

            if (user.IsInRole("Recruiter") && applicant.RecruiterId == userId)
            {
                return View("ForRecruiter", applicant);
            }

            return View();
        }
    }
}
