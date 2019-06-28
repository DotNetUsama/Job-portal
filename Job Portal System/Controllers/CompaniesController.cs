using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;
using Job_Portal_System.Models;
using Job_Portal_System.ViewModels.Companies;
using Job_Portal_System.ViewModels.JobVacancies;
using JW;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Controllers
{
    [Route("Companies")]
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CompaniesController(ApplicationDbContext context,
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("Index")]
        public IActionResult Index(int page)
        {
            var pager = new Pager(_context.Companies.Count(), page, 15);
            var companies = _context.Companies
                .Select(c => new CompanyGeneralViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Logo = c.Logo,
                    FoundedYear = c.FoundedYear,
                    EmployeesNum = c.EmployeesNum,
                    Type = c.Type,
                    DepartmentsNum = _context.CompanyDepartments
                        .Count(d => d.CompanyId == c.Id),
                    OpenedJobsNum = _context.JobVacancies
                        .Count(j =>
                            j.Status == (int)JobVacancyStatus.Open &&
                            j.CompanyDepartment.CompanyId == c.Id),
                    //FrequentJobTitles = _context.JobVacancies
                    //    .GroupBy(j => j.JobTitle.Title)
                    //    .OrderByDescending(gp => gp.Count())
                    //    .Take(5)
                    //    .Select(g => g.Key),
                })
                .OrderBy(c => c.OpenedJobsNum)
                .Skip((pager.CurrentPage - 1) * pager.PageSize)
                .Take(pager.PageSize)
                .ToList();

            var isRecruiter = User.IsInRole("Recruiter");

            return View("CompaniesIndex", new CompaniesIndexViewModel
            {
                IsRecruiter = isRecruiter,
                Companies = companies,
            });
        }

        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .Select(c => new CompanyFullViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Logo = c.Logo,
                    Description = c.Description,
                    EmployeesNum = c.EmployeesNum,
                    FoundedYear = c.FoundedYear,
                    Type = c.Type,
                    Website = c.Website,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Departments = _context.CompanyDepartments
                        .Where(d => d.CompanyId == c.Id)
                        .Select(d => new DepartmentViewModel
                        {
                            State = d.City.State.Name,
                            City = d.City.Name,
                            DetailedAddress = d.DetailedAddress,
                        }),
                    JobVacancies = _context.JobVacancies
                        .Where(j => j.CompanyDepartment.CompanyId == c.Id)
                        .Select(j => new JobVacancyGeneralViewModel
                        {
                            Id = j.Id,
                            Title = j.Title,
                            CreatedAt = j.PublishedAt,
                            IsRemote = j.DistanceLimit == 0,
                            MinSalary = j.MinSalary,
                            MaxSalary = j.MaxSalary,
                            JobTitle = j.JobTitle.Title,
                            Location = $"{j.CompanyDepartment.City.State.Name}, {j.CompanyDepartment.City.Name}",
                            Company = j.CompanyDepartment.Company.Name,
                            DesiredSkills = _context.DesiredSkills
                                .Where(s => s.JobVacancyId == j.Id)
                                .Select(s => s.Skill.Title),
                        }),
                })
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Administrator"))
            {
                company.CanEdit = true;
            }
            else
            {
                var userId = _userManager.GetUserId(User);
                var recruiterInDb = await _context.Recruiters
                    .FirstOrDefaultAsync(recruiter => recruiter.UserId == userId);
                company.CanEdit = recruiterInDb != null && company.Id == recruiterInDb.CompanyId;
            }

            return View("CompanyDetails", company);
        }
    }
}