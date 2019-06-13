using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Areas.Companies.InputModels;
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

        [BindProperty]
        public List<DepartmentInputModel> Departments { get; set; }
        public DepartmentInputModel Department { get; set; }

        [BindProperty]
        public List<DepartmentEditModel> DepartmentsEdits { get; set; }
        
        public Company Company { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await GetCompany(id);

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
            await GetCompany(id);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Company == null) return BadRequest();

            DepartmentsEdits.ForEach(EditDepartment);
            Departments?.ForEach(education => AddDepartment(Company, education));
            Company.EmployeesNum = Company.EmployeesNum;
            Company.Email = Company.Email;
            Company.Website = Company.Website;
            Company.Description = Company.Description;

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

        private async Task GetCompany(string id)
        {
            Company = await _context.Companies
                .Include(c => c.Departments).ThenInclude(d => d.City)
                .FirstOrDefaultAsync(c => c.Id == id);
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