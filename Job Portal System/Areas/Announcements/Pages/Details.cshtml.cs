﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Job_Portal_System.Models;
using Job_Portal_System.Data;
using Microsoft.AspNetCore.Identity;

namespace Job_Portal_System.Areas.Announcements.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DetailsModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Announcement Announcement { get; set; }

        public bool IsOwner { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Announcement = await _context.Announcements
                .Include(a => a.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Announcement == null)
            {
                return NotFound();
            }
            

            var currentUser = await _userManager.GetUserAsync(User);

            IsOwner = currentUser != null && currentUser.Id == Announcement.UserId;

            return Page();
        }
    }
}
