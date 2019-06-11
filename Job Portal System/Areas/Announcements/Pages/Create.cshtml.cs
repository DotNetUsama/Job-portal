using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Job_Portal_System.Areas.Announcements.Pages
{
    [Authorize(Roles = "Administrator")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Announcement Announcement { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Announcement.Author = await _userManager.GetUserAsync(User);
            _context.Announcements.Add(Announcement);
            await _context.SaveChangesAsync();

            return Redirect("Dashboard");
        }
    }
}