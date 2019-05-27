using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Job_Portal_System.Pages
{
    public class TestModel : PageModel
    {
        [BindProperty]
        [Required]
        [Display(Name = "State")]
        public string State { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        public void OnGet()
        {

        }

        public void OnPostPost()
        {

        }
    }
}