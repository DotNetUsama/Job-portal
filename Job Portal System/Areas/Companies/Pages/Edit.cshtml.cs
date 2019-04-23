using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Areas.Companies.Pages
{
    [Authorize(Roles = "Recruiter, Administrator")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Company Company { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Company = await _context.Companies.FirstOrDefaultAsync(company => company.Id == id);

            if (Company == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Administrator")) return Page();

            var recruiterInDb = await _context.Recruiters
                .SingleOrDefaultAsync(recruiter => recruiter.User.UserName == User.Identity.Name);
            if (recruiterInDb == null || Company.Id != recruiterInDb.CompanyId)
            {
                return BadRequest();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(Company.Id))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToPage("./Index");
        }

        private bool CompanyExists(string id)
        {
            return _context.Companies.Any(company => company.Id == id);
        }
    }
}