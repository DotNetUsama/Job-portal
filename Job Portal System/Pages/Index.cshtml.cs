using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Job_Portal_System.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public enum SearchOption
        {
            JobVacancies,
            Resumes,
        }

        [BindProperty]
        [Required]
        public string Query { get; set; }

        [BindProperty]
        public SearchOption Option { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPostSearch()
        {
            switch (Option)
            {
                case SearchOption.JobVacancies:
                    return RedirectToAction("Search", "JobVacancies", new { query = Query });
                case SearchOption.Resumes:
                    return RedirectToAction("Search", "Resumes", new {query = Query});
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
