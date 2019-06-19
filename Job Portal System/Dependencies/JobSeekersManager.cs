using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Dependencies
{
    public class JobSeekersManager
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public JobSeekersManager(ApplicationDbContext context, 
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<JobTitle>> GetSeekedJobTitles(ClaimsPrincipal claim)
        {
            var user = await _userManager.GetUserAsync(claim);
            return _context.Resumes
                .Include(r => r.SeekedJobTitles).ThenInclude(j => j.JobTitle)
                .FirstOrDefault(r => r.UserId == user.Id)?
                .SeekedJobTitles
                .Select(j => j.JobTitle);
        }
    }
}
