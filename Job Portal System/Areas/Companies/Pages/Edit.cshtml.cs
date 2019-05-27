using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Authorization;
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

        public class DepartmentInputModel
        {
            [Required]
            [Display(Name = "State")]
            public string State { get; set; }
            
            [Required]
            [Display(Name = "City")]
            public string City { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [StringLength(255)]
            [Display(Name = "Detailed address")]
            public string DetailedAddress { get; set; }
        }

        public class DepartmentEditModel
        {
            [Required]
            [HiddenInput]
            public string Id { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [StringLength(255)]
            [Display(Name = "Address")]
            public string DetailedAddress { get; set; }
        }

        [BindProperty]
        public List<DepartmentInputModel> Departments { get; set; } = new List<DepartmentInputModel>
        {
            new DepartmentInputModel()
        };

        [BindProperty]
        public List<DepartmentEditModel> DepartmentsEdits { get; set; }

        [BindProperty]
        public Company Company { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Company = await _context.Companies
                .Include(c => c.Departments).ThenInclude(d => d.City)
                .FirstOrDefaultAsync(company => company.Id == id);

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

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var company = _context.Companies
                .Include(c => c.Departments)
                .SingleOrDefault(c => c.Id == id);

            if (company == null) return BadRequest();

            DepartmentsEdits.ForEach(EditDepartment);
            Departments.RemoveAt(Departments.Count - 1);
            Departments.ForEach(education => AddDepartment(company, education));
            company.EmployeesNum = Company.EmployeesNum;
            company.Email = Company.Email;
            company.Website = Company.Website;
            company.Description = Company.Description;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private void EditDepartment(DepartmentEditModel departmentEdit)
        {
            var department = _context.CompanyDepartments.SingleOrDefault(s => s.Id == departmentEdit.Id);
            if (department != null)
            {
                department.DetailedAddress = departmentEdit.DetailedAddress;
            }
        }

        private static void AddDepartment(Company company, DepartmentInputModel department)
        {
            company.Departments.Add(new CompanyDepartment
            {
                CityId = department.City,
                DetailedAddress = department.DetailedAddress,
            });
        }
    }
}