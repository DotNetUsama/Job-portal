using System.Linq;
using Job_Portal_System.Data;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_System.Controllers
{
    [Route("AutoComplete")]
    public class AutoCompleteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AutoCompleteController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Companies")]
        public IActionResult Companies(string term)
        {
            return Json(_context.Companies
                .Where(company => company.Name.Contains(term))
                .Select(company => new
                {
                    company.Id,
                    Label = company.Name,
                })
                .Take(20));
        }

        [HttpGet]
        [Route("CompanyDepartments")]
        public IActionResult CompanyDepartments(string companyId, string term)
        {
            return Json(_context.CompanyDepartments
                .Where(department => department.CompanyId == companyId &&
                                     $"{department.City.Name} {department.DetailedAddress}".Contains(term))
                .Select(department => new
                {
                    department.Id,
                    Label = $"{department.City.Name} ({department.DetailedAddress})",
                }));
        }

        [HttpGet]
        [Route("Skills")]
        public IActionResult Skills(string term)
        {
            return Json(_context.Skills
                .Where(skill => skill.Title.Contains(term))
                .Select(skill => new
                {
                    skill.Id,
                    Label = skill.Title,
                })
                .Take(20));
        }

        [HttpPost]
        [Route("States")]
        public IActionResult States()
        {
            return Json(_context.States
                .Select(state => new
                {
                    state.Id,
                    Label = state.Name,
                }));
        }

        [HttpPost]
        [Route("Cities")]
        public IActionResult Cities(string stateId)
        {
            return Json(_context.Cities
                .Where(city => city.StateId == stateId)
                .Select(city => new
                {
                    city.Id,
                    Label = city.Name,
                }));
        }

        [HttpGet]
        [Route("Schools")]
        public IActionResult Schools(string term)
        {
            return Json(_context.Schools
                .Where(school => school.Name.Contains(term))
                .Select(school => new
                {
                    school.Id,
                    Label = school.Name,
                })
                .Take(20));
        }

        [HttpGet]
        [Route("FieldsOfStudy")]
        public IActionResult FieldsOfStudy(string term)
        {
            return Json(_context.FieldOfStudies
                .Where(fieldOfStudy => fieldOfStudy.Title.Contains(term))
                .Select(fieldOfStudy => new
                {
                    fieldOfStudy.Id,
                    Label = fieldOfStudy.Title,
                })
                .Take(20));
        }

        [HttpGet]
        [Route("JobTitles")]
        public IActionResult JobTitles(string term)
        {
            return Json(_context.JobTitles
                .Where(jobTitle => jobTitle.Title.Contains(term))
                .Select(jobTitle => new
                {
                    jobTitle.Id,
                    Label = jobTitle.Title,
                })
                .Take(20));
        }
    }
}