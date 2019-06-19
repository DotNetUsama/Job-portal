using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Dependencies
{
    public class AnnouncementsManager
    {
        private readonly ApplicationDbContext _context;

        public AnnouncementsManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Announcement>> LatestAsync()
        {
            return await _context.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .ToListAsync();
        }
    }
}
