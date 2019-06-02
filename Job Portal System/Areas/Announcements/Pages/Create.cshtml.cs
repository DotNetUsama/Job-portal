using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly IHostingEnvironment _env;

        public CreateModel(ApplicationDbContext context, UserManager<User> userManager, IHostingEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Announcement Announcement { get; set; }

        [Required]
        [Display(Name = "Image")]
        public IFormFile Image { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var fileName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(Image.FileName));
            var filePath = Path.Combine(_env.WebRootPath, "images", "announcements", fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await Image.CopyToAsync(fileStream);
            }

            Announcement.Image = fileName;
            Announcement.Author = await _userManager.GetUserAsync(User);
            _context.Announcements.Add(Announcement);
            await _context.SaveChangesAsync();

            return Redirect("Dashboard");
        }
    }
}