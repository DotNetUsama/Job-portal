using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Areas.Announcements.Pages
{
    [Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public EditModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Announcement Announcement { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Announcement = await _context.Announcements.FirstOrDefaultAsync(m => m.Id == id);

            if (Announcement == null)
            {
                return NotFound();
            }

            if (Announcement.UserId != _userManager.GetUserId(User)) 
            {
                return BadRequest();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var announcementInDb = await _context.Announcements.FirstOrDefaultAsync(m => m.Id == id);

            if (Announcement == null)
            {
                return NotFound();
            }

            if (Announcement.UserId != _userManager.GetUserId(User))
            {
                return BadRequest();
            }

            if (announcementInDb.Content != Announcement.Content)
            {
                announcementInDb.Content = announcementInDb.Content;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}